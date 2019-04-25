<template>
  <div>
  <el-container>
    <el-header style="text-align: left; font-size: 18px">
      <span>Contract {{methodType}} Interface for {{contractFile.name}}{{deploymentInfo}}</span>
    </el-header>
    <el-main>
      <contract-method v-for="method in methods" :key="method.name" :prod="prod" :owner="owner" :contractFile="contractFile" :method="method" :contractAddress="contractAddress" @onConstructorExecuted="onConstructorExecuted"/>
    </el-main>
  </el-container>
  <el-dialog title="Create Contract" :visible.sync="dialogFormVisible">
    <el-form ref="form" :model="form" :rules="rules" v-loading="loading">
      <div v-if="active === 0" class="form-div">
        <span>A registration fee must be submitted in order to register new contracts with this service. Do you still want to deploy this contract?</span>
      </div>
      <div v-if="active === 2" class="form-div">
        <el-form-item label="Sender" prop="sender" :label-width="formLabelWidthSmall">
          <el-input v-model="owner" autoComplete="off" :readonly="true"></el-input>
        </el-form-item>
        <el-form-item label="Recipient" prop="recipient" :label-width="formLabelWidthSmall">
          <el-input v-model="form.recipient" autoComplete="off" :readonly="true"></el-input>
        </el-form-item>
        <el-form-item label="Amount" prop="feeAmount" :label-width="formLabelWidthSmall">
          <el-input v-model="form.feeAmount" autoComplete="off" :readonly="true"></el-input>
        </el-form-item>
      </div>
      <div v-if="active === 1" class="form-div">
        <el-row>
          <el-col :span="12">
            <el-form-item label="Name" prop="name" :label-width="formLabelWidthSmall">
              <el-input v-model="form.name" placeholder="Contract name" autoComplete="off"></el-input>
            </el-form-item>
            <el-form-item label="Expiration Date/Time (UTC)" prop="expirationDateTime" :label-width="formLabelWidthLarge">
              <el-tooltip placement="bottom" effect="light" content="Contract lifetime on this platform. Should be after contract expiration date, e.g. settlement deadline.">
                <el-date-picker
                  v-model="form.expirationDateTime"
                  type="datetime"
                  placeholder="Select date and time"
                  default-time="12:00:00">
                </el-date-picker>
              </el-tooltip>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="Conditition Measure" prop="measure" :label-width="formLabelWidthLarge">
              <el-select v-model="form.measure" placeholder="Select weather condition measure">
                <el-option label="Temperature" value="Temperature"></el-option>
                <el-option label="Precipitation" value="Precipitation"></el-option>
                <el-option label="Humidity" value="Humidity"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item label="Pay Out Trigger Type" prop="isHigh" :label-width="formLabelWidthLarge">
              <el-tooltip placement="bottom" effect="light">
                <div slot="content">Higher: Pay out if actual condition measure at expiration is above contract condition measure.<br/>Lower: Pay out if actual condition measure at expiration is below contract condition measure.</div>
                <el-switch
                  v-model="form.isHigh"
                  active-text="Higher"
                  inactive-text="Lower">
                </el-switch>
              </el-tooltip>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="24">
            <el-form-item label="Contract Description" prop="description" :label-width="formLabelWidthLarge">
              <el-input v-model="form.description" type="textarea" placeholder="Contract description (exact location, measurement unit, settlement proceduce etc)" autoComplete="off" :autosize="{ minRows: 6, maxRows: 6}"></el-input>
            </el-form-item>
          </el-col>
        </el-row>
      </div>
    </el-form>
    <el-steps :active="active" align-center finish-status="success">
      <el-step title="Deploy Contract"></el-step>
      <el-step title="Register Contract"></el-step>
      <el-step title="Pay Fee"></el-step>
    </el-steps>
    <span slot="footer" class="dialog-footer">
      <el-button @click="dialogFormVisible = false">Cancel</el-button>
      <el-button type="primary" @click="next">Confirm</el-button>
    </span>
  </el-dialog>
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
  .form-div {
    height: 380px;
  }
</style>

<script>
import ContractMethod from '../components/ContractMethod'
import { serverBus } from '../main'

export default {
  components: { ContractMethod },
  props: [ 'prod', 'owner', 'contractFile', 'methodType', 'contractAddress' ],
  data () {
    return {
      dialogFormVisible: false,
      loading: false,
      active: 0,
      constructorParameters: [],
      createdContractAddress: null,
      fee: null,
      form: {
        name: null,
        measure: '',
        isHigh: true,
        expirationDateTime: null,
        description: null,
        feeAmount: null
      },
      formLabelWidthSmall: '120px',
      formLabelWidthLarge: '200px',
      rules: {
        name: [
          { validator: this.validateName, trigger: 'blur' },
          { required: true, min: 7, max: 25, message: 'Length should be between 7 and 25 characters.', trigger: 'blur' }
        ],
        description: [
          { required: true, message: 'Please provide a contract description.', trigger: 'blur' }
        ],
        expirationDateTime: [
          { type: 'date', required: true, message: 'Please pick a date.', trigger: 'change' }
        ],
        measure: [
          { required: true, message: 'Please select contract condition measure.', trigger: 'change' }
        ]
      }
    }
  },
  created () {
    // Using the server bus
    serverBus.$on('contractChanged', () => {
      this.constructorParameters = []
      this.createdContractAddress = null
      this.fee = null
      this.form.symbol = null
    })
  },
  computed: {
    methods: function () {
      const methods = this.contractFile.abi.filter(i => i.type === this.methodType.toLowerCase())
      return methods
    },
    deploymentInfo: function () {
      if (this.contractAddress) {
        return ` deployed at ${this.contractAddress}`
      }
      return ''
    }
  },
  methods: {
    async validateName (rule, value, callback) {
      if (!value || value === '') {
        callback(new Error('Please enter a contract name'))
      }
      const validationResult = await this.prod.validateName(this.form.name)
      if (validationResult !== '') {
        callback(new Error(validationResult))
      } else {
        callback()
      }
    },
    async onConstructorExecuted (parameters) {
      this.constructorParameters = parameters
      this.active = 0
      this.dialogFormVisible = true
    },
    async next () {
      let success = false
      if (this.active === 0) {
        success = await this.deployContract()
      }
      if (this.active === 1) {
        success = await this.registerContract()
      }
      if (this.active === 2) {
        success = await this.payFee()
        if (success) this.dialogFormVisible = false
        this.$emit('onContractCreated', this.contractFile.apiType, this.createdContractAddress)
      }
      if (success && this.active++ > 2) this.active = 0
    },
    async deployContract () {
      try {
        this.loading = true
        const newContractAddress = await this.prod.createContract(this.owner, this.contractFile.abi, this.contractFile.bytecode, this.constructorParameters)
        this.createdContractAddress = newContractAddress
        this.loading = false
        return true
      } catch (ex) {
        this.$message({
          showClose: true,
          message: ex.message,
          duration: 8000,
          type: 'error'
        })
        this.loading = false
        return false
      }
    },
    async registerContract () {
      this.loading = true
      const isValid = await this.$refs.form.validate()
      if (!isValid) return false
      const contract = {
        address: this.createdContractAddress,
        ownerAddress: this.owner,
        contractFile: this.contractFile,
        network: this.prod.network,
        name: this.form.name,
        contractType: this.getContractType(this.form.measure, this.form.isHigh),
        expirationDateTime: this.form.expirationDateTime.toLocaleString(),
        description: this.form.description,
        constructorArguments: JSON.stringify(this.constructorParameters)
      }
      await this.prod.post('deployedcontract', contract, this.owner)
      const fee = await this.prod.get(`calculatecontractfee/${contract.address}`)
      this.form.recipient = fee.recipient
      this.form.feeAmount = fee.amount
      this.fee = fee
      this.loading = false
      return true
    },
    getContractType (measure, isHigh) {
      if (isHigh) {
        switch (measure) {
          case 'Temperature':
            return 'HighTemperatureInsurance'
          case 'Precipitation':
            return 'HighPrecipitationInsurance'
          case 'Humidity':
            return 'HighHumidityInsurance'
        }
      } else {
        switch (measure) {
          case 'Temperature':
            return 'LowTemperatureInsurance'
          case 'Precipitation':
            return 'LowPrecipitationInsurance'
          case 'Humidity':
            return 'LowHumidityInsurance'
        }
      }
    },
    async payFee () {
      this.loading = true
      const result = await this.prod.transfer(this.owner, this.form.recipient, this.form.feeAmount)
      this.fee.transactionHash = result
      await this.prod.post('fee', this.fee, this.owner)
      this.loading = false
      this.$message(result)
      return true
    }
  }
}
</script>
