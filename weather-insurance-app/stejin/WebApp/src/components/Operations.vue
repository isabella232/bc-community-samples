<template>
  <div>
    <el-container>
    <el-header style="text-align: left; font-size: 18px">
      <span>Ethereum user account</span>
    </el-header>
    <el-main>
      <el-select v-model="selectedUserAccountAddress" placeholder="Select User Account Address" style="width: 400px" @change="handleUserAccountChange">
        <el-option
          v-for="account in userAccounts"
          :key="account.address"
          :label="account.address"
          :value="account">
        </el-option>
      </el-select>
    </el-main>
    </el-container>
    <div v-if="selectedUserAccount">
      <el-container>
        <el-header style="text-align: left; font-size: 18px">
          <span>Contract files associated with <b>{{ selectedUserAccount.address }}</b> ({{ selectedUserAccount.balance }} ETH)</span>
          <span style="float: right">
            <el-button type="primary" size="small" @click="update"><i class="el-icon-refresh"></i></el-button>
          </span>
        </el-header>
        <el-main>
          <el-table
            ref="contractFileTable"
            v-loading="contractFileTableLoading"
            :data="contractFiles"
            highlight-current-row
            @current-change="handleContractFileChange"
            stripe
            style="width: 100%">
            <el-table-column
              prop="name"
              label="File Name"
              sortable
              width="250">
            </el-table-column>
            <el-table-column
              label=".json"
              width="90">
              <template slot-scope="scope">
                <i :class="getIcon(scope.row.includesJson)"></i>
              </template>
            </el-table-column>
            <el-table-column
              label=".sol"
              width="90">
              <template slot-scope="scope">
                <i :class="getIcon(scope.row.includesSol)"></i>
              </template>
            </el-table-column>
            <el-table-column
              prop="apiType"
              label="Type"
              width="200">
            </el-table-column>
            <el-table-column
              prop="apiVersion"
              label="API Version"
              width="150">
            </el-table-column>
            <el-table-column
              prop="description"
              label="Description"
              width="600">
            </el-table-column>
            <el-table-column
              fixed="right"
              label="Operations"
              width="120">
              <template slot-scope="scope">
                <el-dropdown size="small" split-button type="primary" trigger="click" @command="handleContractFileCommand" @click="editContractFile(selectedUserAccount.address, scope.row)">
                  Edit
                  <el-dropdown-menu slot="dropdown">
                    <el-dropdown-item command="edit">Edit</el-dropdown-item>
                    <el-dropdown-item command="delete">Delete</el-dropdown-item>
                    <el-dropdown-item :disabled="!scope.row.includesJson" command="showJson">Abi</el-dropdown-item>
                    <el-dropdown-item :disabled="!scope.row.includesSol" command="showSol">Source</el-dropdown-item>
                  </el-dropdown-menu>
                </el-dropdown>
              </template>
            </el-table-column>
          </el-table>
          <el-dialog
            :title="contractFileContentTitle"
            :visible.sync="showContractFileContent"
            width="60%">
            <span>
              <el-input
                type="textarea"
                :readonly="true"
                :autosize="{minRows: 6, maxRows: 20}"
                placeholder="Loading..."
                v-model="contractFileContent">
              </el-input>
            </span>
            <span slot="footer" class="dialog-footer">
            <el-button type="primary" @click="showContractFileContent = false">Close</el-button>
            </span>
          </el-dialog>
          <el-upload
            class="upload-demo"
            ref="upload"
            accept=".json,.sol"
            :multiple="true"
            :headers="headers"
            :before-upload="addAuthenticationHeader"
            :on-success="updateContractFiles"
            :on-error="handleUploadError"
            :action="apiPath"
            :auto-upload="true">
            <el-button slot="trigger" type="primary">Add Files</el-button>
            <div class="el-upload__tip" slot="tip">Compiled contract file (.json) and corresponding source files (.sol)</div>
          </el-upload>
        </el-main>
      </el-container>
    </div>
    <div v-if="selectedContractFile && selectedContractFile.isComplete">
      <div v-if="(selectedContractFile.apiType === 'WeatherInsurance')">
        <el-container>
          <el-header style="text-align: left; font-size: 18px">
            <span>Deployments of <b>{{ selectedContractFile.name }}</b></span>
          </el-header>
          <el-main>
            <el-table
              ref="weatherInsuranceContractTable"
              key="weatherInsuranceContractTable"
              v-loading="deployedContractsTableLoading"
              :data="deployedWeatherInsuranceContracts"
              highlight-current-row
              @current-change="handleContractChange"
              stripe
              style="width: 100%">
              <el-table-column
                prop="address"
                label="Contract Address"
                sortable
                width="350">
              </el-table-column>
              <el-table-column
                prop="name"
                label="Name"
                sortable
                width="250">
              </el-table-column>
              <!--el-table-column
                prop="type"
                label="Contract Type"
                sortable
                width="210">
              </el-table-column>
              <el-table-column
                prop="location"
                label="Location"
                sortable
                width="160">
              </el-table-column-->
              <el-table-column
                label="Registered"
                width="100">
                <template slot-scope="scope">
                  <i :class="getIcon(scope.row.isRegistered)"></i>
                </template>
              </el-table-column>
              <el-table-column
                label="In Use"
                width="90">
                <template slot-scope="scope">
                  <i :class="getIcon(scope.row.inUse)"></i>
                </template>
              </el-table-column>
              <el-table-column
                prop="balance"
                label="Contract Balance (ETH)"
                sortable
                width="200">
              </el-table-column>
              <el-table-column
                prop="totalNotional"
                label="Total Notional (ETH)"
                sortable
                width="200">
              </el-table-column>
              <el-table-column
                prop="totalPremium"
                label="Premiums (ETH)"
                sortable
                width="200">
              </el-table-column>
              <el-table-column
                prop="totalValue"
                label="Contract Value (ETH)"
                sortable
                width="200">
              </el-table-column>
              <el-table-column
                fixed="right"
                label="Operations"
                width="120">
                <template slot-scope="scope">
                  <el-dropdown size="small" split-button type="primary" trigger="click" @command="handleContractFileCommand" @click="fundContract(selectedUserAccount.address, scope.row)">
                    Fund
                    <el-dropdown-menu slot="dropdown">
                      <el-dropdown-item command="fund">Fund</el-dropdown-item>
                    </el-dropdown-menu>
                  </el-dropdown>
                </template>
              </el-table-column>
            </el-table>
          </el-main>
        </el-container>
      </div>
    </div>
    <div v-if="selectedContractFile && selectedContractFile.isComplete && !selectedContract">
      <contract-interface :prod="prod" :owner="selectedUserAccountAddress" :contractFile="selectedContractFile" methodType="Constructor" @onContractCreated="onContractCreated"/>
    </div>
    <div v-if="selectedContractFile && selectedContractFile.isComplete && selectedContract">
      <contract-interface :prod="prod" :owner="selectedUserAccountAddress" :contractFile="selectedContractFile" methodType="Function" :contractAddress="selectedContract.address" />
    </div>
  </div>
</template>

<style scoped>
  .el-header {
    background-color: #B3C0D1;
    color: #333;
    line-height: 60px;
  }
  .el-main {
    text-align: left;
    padding: 10px 0px 20px 0px;
  }
</style>

<script>
import ContractInterface from '../components/ContractInterface'
import { serverBus } from '../main'

export default {
  components: { ContractInterface },
  props: ['prod'],
  data () {
    return {
      path: 'contractfile',
      userAccounts: [],
      selectedUserAccountAddress: '',
      selectedUserAccount: null,
      headers: null,
      contractFileTableLoading: false,
      contractFiles: [],
      selectedContractFile: null,
      deployedWeatherInsuranceContracts: [],
      deployedContractsTableLoading: false,
      selectedContract: null,
      showContractFileContent: false,
      contractFileContentTitle: '',
      contractFileContent: ''
    }
  },
  computed: {
    apiPath: function () {
      return `${this.prod.apiBasePath}/${this.path}`
    }
  },
  mounted: function () {
    this.prod.getUserAccounts().then(accounts => {
      this.userAccounts = accounts
    })
  },
  methods: {
    getIcon (value) {
      if (value) {
        return 'el-icon-circle-check'
      } else {
        return 'el-icon-circle-close'
      }
    },
    async addAuthenticationHeader (file) {
      if ((file.name.endsWith('.json') && this.contractFiles.filter(f => f.includesJson && `${f.name}.json` === file.name).length > 0) || (file.name.endsWith('.sol') && this.contractFiles.filter(f => f.includesSol && `${f.name}.sol` === file.name).length > 0)) {
        this.$message.error('File alread exists.')
        this.$refs.upload.abort(file)
        return Promise.reject(new Error(400))
      } else {
        this.headers = await this.prod.getHeaders('POST', this.path, this.selectedUserAccount.address, null)
      }
    },
    async updateContractFiles (response, file, fileList) {
      this.contractFiles = await this.prod.getContractFiles(this.selectedUserAccount.address)
    },
    handleUploadError (err, file, fileList) {
      this.$message.error(`File upload failed. ${err}`)
    },
    async handleUserAccountChange (val) {
      this.contractFileTableLoading = true
      this.selectedContractFile = null
      this.selectedContract = null
      if (this.$refs.upload) {
        this.$refs.upload.clearFiles()
      }
      this.selectedUserAccount = val
      if (val) {
        this.prod.getBalance(val.address).then(amount => { this.selectedUserAccount.balance = this.prod.convertToAccountingUnit(amount) })
        this.selectedUserAccountAddress = val.address
        this.contractFiles = await this.prod.getContractFiles(this.selectedUserAccount.address)
        this.contractFiles.sort((a, b) => a.name > b.name ? 1 : -1)
      }
      this.contractFileTableLoading = false
    },
    async update () {
      await this.handleUserAccountChange(this.selectedUserAccount)
    },
    async handleContractFileChange (val) {
      this.deployedContractsTableLoading = true
      this.selectedContract = null
      this.deployedWeatherInsuranceContracts = []
      if (val !== null) {
        this.selectedContractFile = await this.prod.getContractFile(val.id, true, true)
        if (val.apiType === 'WeatherInsurance')
          this.deployedWeatherInsuranceContracts = await this.prod.getDeployedWeatherInsuranceContractsForFileId(val.id, this.selectedUserAccountAddress)
      }
      this.deployedContractsTableLoading = false
    },
    async handleContractChange (val) {
      this.selectedContract = val
      serverBus.$emit('contractChanged')
    },
    handleContractFileCommand (command) {
      switch (command) {
        case 'edit':
          this.editContractFile(this.selectedUserAccount.address, this.selectedContractFile)
          break
        case 'delete':
          this.deleteContractFile(this.selectedUserAccount.address, this.selectedContractFile.id)
          break
        case 'showJson':
          this.showJson()
          break
        case 'showSol':
          this.showSol()
          break
      }
    },
    editContractFile (user, contractFile) {
      const oldDescription = contractFile.description
      this.$prompt('Please enter file description (max 256 characters)', 'Edit Contract File', {
        confirmButtonText: 'OK',
        cancelButtonText: 'Cancel',
        inputPattern: /^.*$/,
        inputErrorMessage: 'Invalid input'
      }).then(value => {
        this.selectedContractFile.description = value.value
        this.prod.updateContractFile(user, this.selectedContractFile).then(res => {
          this.$message({
            type: 'success',
            message: `Updated ${contractFile.name}`
          })
          contractFile.description = value.value
        })
      }).catch((ex) => {
        this.selectedContractFile.description = oldDescription
        this.$message({
          type: 'error',
          message: 'ex'
        })
      })
    },
    deleteContractFile (user, contractFileId) {
      this.$confirm('This will delete contract file. Continue?', 'Warning', {
        confirmButtonText: 'OK',
        cancelButtonText: 'Cancel',
        type: 'warning'
      }).then(() => {
        this.prod.deleteContractFile(user, contractFileId).then(result => {
          this.$message({
            type: 'success',
            message: 'Delete complete'
          })
          this.contractFiles = this.contractFiles.filter(i => i.id !== contractFileId)
          this.selectedContractFile = null
          this.selectedContract = null
        })
      }).catch(() => {
        this.$message({
          type: 'info',
          message: 'Delete canceled'
        })
      })
    },
    async onContractCreated (contractType, contractAddress) {
      this.deployedContractsTableLoading = true
      if (contractType === 'WeatherInsurance')
        this.deployedWeatherInsuranceContracts = await this.prod.getDeployedWeatherInsuranceContractsForFileId(this.selectedContractFile.id, this.selectedUserAccountAddress)
      this.deployedContractsTableLoading = false
    },
    showJson () {
      if (this.selectedContractFile.includesJson) {
        this.contractFileContentTitle = 'Abi and Bytecode'
        this.contractFileContent = `Abi: ${JSON.stringify(this.selectedContractFile.abi)}\r\n\r\nBytecode: ${this.selectedContractFile.bytecode}`
        this.showContractFileContent = true
      }
    },
    showSol () {
      if (this.selectedContractFile.includesSol) {
        this.contractFileContentTitle = 'Source Code'
        this.contractFileContent = `${this.selectedContractFile.sourceCode}`
        this.showContractFileContent = true
      }
    },
    handleDeployedContractCommand (command) {
      switch (command) {
        case 'fund':
          this.fundContract(this.selectedUserAccount.address, this.selectedContract)
          break
      }
    },
    fundContract (userAddress, contract) {
      this.$prompt('Please enter amount to send in ETH', 'Fund contract', {
        confirmButtonText: 'OK',
        cancelButtonText: 'Cancel',
        inputPattern: /^[+]?([0-9]+(?:[.][0-9]*)?|\.[0-9]+)$/,
        inputErrorMessage: 'Invalid amount'
      }).then(value => {
        this.prod.transfer(userAddress, contract.address, this.prod.convertToBaseUnit(value.value)).then(res => {
          this.$message({
            type: 'success',
            message: `Sent ${value.value} ETH to contract at ${contract.address}`
          })
          this.prod.getBalance(userAddress).then(amount => { this.selectedUserAccount.balance = this.prod.convertToAccountingUnit(amount) })
          this.prod.getBalance(contract.address).then(amount => { this.selectedContract.balance = this.prod.convertToAccountingUnit(amount) })
        })
      }).catch(() => {
        this.$message({
          type: 'info',
          message: 'Input canceled'
        })
      })
    }
  }
}
</script>
