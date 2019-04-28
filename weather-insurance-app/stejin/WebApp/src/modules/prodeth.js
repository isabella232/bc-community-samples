import Eth from 'ethjs'
import EthContract from 'ethjs-contract'
import EthFilter from 'ethjs-filter'
import EthQuery from 'ethjs-query'

const Actions = {
  GET: 'GET',
  POST: 'POST',
  PUT: 'PUT',
  DELETE: 'DELETE'
}

class ProD {
  constructor (http) {
    if (typeof web3 !== 'undefined') {
      // eslint-disable-next-line
      web3.currentProvider.enable()
      // eslint-disable-next-line
      this.eth = new Eth(web3.currentProvider)
    } else {
      this.eth = new Eth(new Eth.HttpProvider('http://localhost:8545'))
    }
    this.contract = new EthContract(this.eth)
    this.filters = new EthFilter(this.eth)
    this.query = new EthQuery(this.eth)
    this.http = http
    this.apiBasePath = 'https://dim.azurewebsites.net' // 'http://localhost:7071' 
  }

  async initialize () {
    this.network = await this.getConnectedNetwork()
    this.referenceWeatherInsuranceContractFile = await this.getReferenceContractFile('WeatherInsurance')
  }

  getTicks () {
    const d = new Date()
    const n = d.getTime()
    return n
  }

  async sign (signerAddress, message) {
    // eslint-disable-next-line
    if (typeof web3 !== 'undefined') {
      return new Promise(resolve => {
        // eslint-disable-next-line
        web3.personal.sign(message, signerAddress, (err, result) => {
          resolve(result)
        })
      })
    } else {
      const hexmessage = Eth.fromUtf8(message)
      const signature = await this.eth.sign(signerAddress, hexmessage)
      return signature
    }
  }

  async getHeaders (action, path, signerAddress, data) {
    const nonce = this.getTicks()
    let message = `${nonce}|${action}|/${path}|`
    if (data !== null) {
      message += JSON.stringify(data)
    }
    const signature = await this.sign(signerAddress, message)
    const token = {
      PublicKey: signerAddress,
      Signature: signature,
      Timestamp: new Date().toISOString(),
      Nonce: nonce
    }
    const headers = {
      AsymmetricAuthentication: JSON.stringify(token)
    }
    return headers
  }

  async callApi (action, path, signerAddress = null, data = null) {
    const fullPath = `${this.apiBasePath}/${path}`
    let headers = {}
    if (signerAddress != null) {
      headers = await this.getHeaders(action, path, signerAddress, data)
    }
    let result
    switch (action) {
      case Actions.GET:
        result = (await this.http.get(fullPath, { headers: headers })).body
        break
      case Actions.POST:
        result = await this.http.post(fullPath, data, { headers: headers })
        break
      case Actions.PUT:
        result = await this.http.put(fullPath, data, { headers: headers })
        break
      case Actions.DELETE:
        result = await this.http.delete(fullPath, { headers: headers })
        break
    }
    return result
  }

  async get (path, userAccountAddress = null) {
    const result = await this.callApi(Actions.GET, path, userAccountAddress)
    return result
  }

  async post (path, entity, userAccountAddress = null) {
    const result = await this.callApi(Actions.POST, path, userAccountAddress, entity)
    return result
  }

  async put (path, entity, userAccountAddress = null) {
    const result = await this.callApi(Actions.PUT, path, userAccountAddress, entity)
    return result
  }

  async delete (path, userAccountAddress) {
    const result = await this.callApi(Actions.DELETE, path, userAccountAddress)
    return result
  }

  startBlockWatch (cb) {
    const filter = new this.filters.BlockFilter({delay: 5000})
    filter.new().then((result) => {
      // console.log(result)
    }).catch((ignore) => {
      // console.log(error)
    })
    const watcher = filter.watch((error, result) => {
      if (error) {
        console.log(error)
      }
      if (result) {
        cb()
      }
    })
    return watcher
  }

  startListeningForInsuranceBoughtEvents (contractAddress, cb) {
    const contract = this.getWeatherInsuranceContractApi(contractAddress)
    const filter = contract.InsuranceBought()
    filter.new({ toBlock: 'latest', address: contractAddress, to: undefined })
    const watcher = filter.watch((err, result) => {
      if (err) {
        console.log(err)
      }
      if (result[0]) {
        cb(result[0].data)
      }
    })
    return watcher
  }

  stopWatch (watcher) {
    watcher.stopWatching()
  }

  async getNetworks () {
    const result = await this.callApi(Actions.GET, `networks`)
    return result
  }

  async getConnectedNetwork () {
    const networks = await this.getNetworks()
    const defaultNetwork = networks.filter(n => n.name === 'ETH:Unknown')[0]
    const filteredNetworks = networks.filter(n => n.name !== 'ETH:Unknown')
    for (const n in filteredNetworks) {
      const network = filteredNetworks[n]
      const code = await this.eth.getCode(network.referenceContractAddress)
      if (code !== '0x') {
        return network
      }
    }
    return defaultNetwork
  }

  async getReferenceContractFileAsync (contractType) {
    const result = await this.callApi(Actions.GET, `contractFiles/reference/${contractType}`)
    return result
  }

  getReferenceContractFile (contractType) {
    return new Promise(resolve => {
      this.getReferenceContractFileAsync(contractType).then(result => {
        resolve(result)
      })
    })
  }

  getUserAccounts () {
    return new Promise(resolve => {
      this.eth.accounts((accountsError, accounts) => {
        const result = []
        if (accountsError) {
          console.log(accountsError)
        }
        for (const a in accounts) {
          const account = accounts[a]
          this.getBalance(account).then(bal => {
            const balance = this.amountAsEther(bal)
            const r = {
              address: account,
              balance: balance
            }
            result.push(r)
          })
        }
        resolve(result)
      })
    })
  }

  getBalance (address) {
    return new Promise(resolve => {
      this.eth.getBalance(address, (err, bal) => {
        if (err) {
          console.log(err)
          resolve(0)
        }
        resolve(bal)
      })
    })
  }

  async transfer (from, to, amount) {
    if (typeof web3 !== 'undefined') {
      return new Promise(resolve => {
        // eslint-disable-next-line
        web3.eth.sendTransaction({data: '', from: from, to: to, value: amount}, (err, result) => {
          resolve(result)
        })
      })
    } else {
      const result = await this.eth.sendTransaction({data: '', from: from, to: to, value: amount})
      return result
    }
  }

  async executeContractFunction (userAccountAddress, contractAddress, abi, method, params = []) {
    let result = null
    const Contract = this.contract(abi)
    const contract = Contract.at(contractAddress)
    result = await contract[method.name](...params, {from: userAccountAddress})
    return result
  }

  sleep (ms) {
    return new Promise(resolve => setTimeout(resolve, ms))
  }

  amountAsWei (amount) {
    return Eth.fromWei(amount, 'wei')
  }

  amountAsEther (amount) {
    return Eth.fromWei(amount, 'ether')
  }

  convertToAccountingUnit (amount) {
    return this.amountAsEther(amount)
  }

  convertToBaseUnit (amount) {
    return Eth.toWei(amount, 'ether')
  }

  async getTransactionResult (txh) {
    let tx = await this.getTransactionReceipt(txh)
    while (!tx) {
      console.log('Waiting for transaction to complete')
      await this.sleep(3000)
      tx = await this.getTransactionReceipt(txh)
    }
    return {contractAddress: tx.contractAddress, transactionHash: tx.transactionHash, status: parseInt(tx.status)}
  }

  async getContractFiles (userAccount) {
    const contractFiles = await this.callApi(Actions.GET, `contractfiles`, userAccount)
    return contractFiles
  }

  async getContractFile (fileId, includeJson = false, includeSol = false) {
    const includedItems = []
    if (includeJson) {
      includedItems.push('abi')
      includedItems.push('bytecode')
    }
    if (includeSol) {
      includedItems.push('source')
    }
    const contractFile = await this.callApi(Actions.GET, `contractfile/?id=${fileId}&include=${includedItems.join(',')}`)
    return contractFile
  }

  async updateContractFile (userAccount, contractFile) {
    const result = await this.callApi(Actions.PUT, `contractfile`, userAccount, contractFile)
    return result
  }

  async deleteContractFile (userAccount, contractFileId) {
    const result = await this.callApi(Actions.DELETE, `contractfile/?id=${contractFileId}`, userAccount)
    return result
  }

  async getRegisteredWeatherInsuranceContractsForApiType (apiType, activeOnly, userAccount) {
    const result = []
    let deployedContracts = []
    
    if (activeOnly)
      deployedContracts = await this.callApi(Actions.GET, `registeredcontracts/${this.network.id}?apitype=${apiType}&isactive=true`)
    else
      deployedContracts = await this.callApi(Actions.GET, `registeredcontracts/${this.network.id}?apitype=${apiType}`)

    for (const i in deployedContracts) {
      const deployedContract = deployedContracts[i]
      const r = await this.getPublicDeployedWeatherInsuranceContractDetails(deployedContract, userAccount)
      result.push(r)
    }
    return result
  }

  async getDeployedWeatherInsuranceContractsForFileId (contractFileId, userAccount) {
    const result = []
    const deployedContracts = await this.callApi(Actions.GET, `deployedcontracts/?fileid=${contractFileId}`, userAccount)
    for (const i in deployedContracts) {
      const deployedContract = deployedContracts[i]
      const r = await this.getDeployedWeatherInsuranceContractDetails(deployedContract, userAccount)
      result.push(r)
    }
    return result
  }

  async getPublicDeployedWeatherInsuranceContractDetails (deployedContract, userAddress) {
    const contract = this.getWeatherInsuranceContractApi(deployedContract.address)
    const balance = await this.getBalance(deployedContract.address)
    const owner = (await contract.owner())[0]
    let association = 'User'
    let notional = Eth.toBN(0)
    let premium = Eth.toBN(0)
    if (userAddress) {
      const isOwner = owner === userAddress
      const isOperator = (await contract.operators(userAddress))[0]
      association = isOwner ? 'Owner' : isOperator ? 'Operator' : 'Observer'
      const position = await contract.getPosition(userAddress)
      notional = position[0]
      premium = position[1]
    }
    const location = (await contract.location())[0]
    const condition = (await contract.condition())[0]
    const expirationTime = (await contract.expirationTime())[0]
    const valuationTime = (await contract.valuationTime())[0]
    const minimumPremium = (await contract.minimumPremium())[0]
    const forecast = (await contract.forecast())[0]
    const forecastRisk = (await contract.forecastRisk())[0]
    const r = {
      address: deployedContract.address,
      ownerAddress: owner,
      name: deployedContract.name,
      type: deployedContract.contractType,
      expirationDateTime: deployedContract.expirationDateTime,
      expirationTime: (new Date(parseInt(expirationTime) * 1000)).toLocaleString(),
      valuationTime: (new Date(parseInt(valuationTime) * 1000)).toLocaleString(),
      description: deployedContract.description,
      balance: this.amountAsEther(balance),
      association: association,
      location: location,
      condition: parseInt(condition),
      minimumPremium: this.amountAsEther(minimumPremium),
      forecast: parseInt(forecast),
      forecastRisk: parseInt(forecastRisk),
      isRegistered: deployedContract.isRegistered,
      notional: this.amountAsEther(notional),
      premium: this.amountAsEther(premium),
      contractFileId: deployedContract.contractFile.id,
      hasCompiledCode: deployedContract.contractFile.includesJson,
      hasSourceCode: deployedContract.contractFile.includesSol
    }
    return r
  }

  async getDeployedWeatherInsuranceContractDetails (deployedContract, ownerOperator) {
    const r = await this.getPublicDeployedWeatherInsuranceContractDetails(deployedContract, ownerOperator)
    const contract = this.getWeatherInsuranceContractApi(deployedContract.address)
    const users = (await contract.getUsers({from: ownerOperator}))[0]
    let totalNotional = 0
    let totalPremium = 0
    let totalValue = 0
    for (const u in users) {
      const user = users[u]
      const position = await contract.getPosition(user)
      totalNotional += parseFloat(this.amountAsEther(position[0]))
      totalPremium += parseFloat(this.amountAsEther(position[1]))
      totalValue += parseFloat(this.amountAsEther((await contract.getIntrinsicValue(position[0]))[0]))
    }
    // const totalNotional = positions.reduce((a, b) => { return a[0] + b[0] }, Eth.toBN(0))
    // const totalPremium = positions.reduce((a, b) => { return a[1] + b[1] }, 0)
    // const totalValue = positions.reduce(async (a, b) => { return (await contract.getIntrinsicValue(a[0]))[0] + (await contract.getIntrinsicValue(b[0]))[0] }, 0)  
    r.inUse = users.length > 0
    r.userCount = users.length
    r.totalNotional = totalNotional
    r.totalPremium = totalPremium
    r.totalValue = totalValue
    return r
  }

  getWeatherInsuranceContractApi (contractAddress) {  
    const abi = this.referenceWeatherInsuranceContractFile.abi
    const DeployedContract = this.contract(abi)
    const contract = DeployedContract.at(contractAddress)
    return contract
  }

  async validateName (name) {
    const hexName = Eth.fromUtf8(name)
    const result = await this.get(`validateContractName/${this.network.id}?name=${hexName}`)
    return result
  }

  async createContract (owner, abi, bytecode, params = []) {
    const txh = await this.newContract(owner, abi, bytecode, params)
    const tx = await this.getTransactionResult(txh)
    return tx.contractAddress
  }

  newContract (owner, abi, bytecode, params = []) {
    const Contract = this.contract(abi, bytecode, {
      from: owner,
      gas: 5000000
    })
    return new Promise(resolve => {
      Contract.new(...params, (err, txx) => {
        if (err) {
          console.log(err)
        }
        resolve(txx)
      })
    })
  }

  async getTransactionReceipt (transactionHash) {
    const result = await this.eth.getTransactionReceipt(transactionHash)
    return result
  }

  async getPremium (contractAddress, notional) {
    const contract = this.getWeatherInsuranceContractApi(contractAddress)
    const result = (await contract.getPremium(notional))[0]
    return result
  }

  async buyInsurance (userAddress, contractAddress, notional, premium) {
    const contract = this.getWeatherInsuranceContractApi(contractAddress)
    const txh = await contract.buyInsurance(notional, {from: userAddress, value: premium, gas: 5000000})
    const result = await this.getTransactionResult(txh)
    return result
  }

}

export default {
  ProD
}