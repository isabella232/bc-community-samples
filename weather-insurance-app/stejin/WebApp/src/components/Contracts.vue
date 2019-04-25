<template>
  <el-container>
    <el-aside width="160px">
      <el-menu default-active="WeatherInsurance" @select="contractTypeChanged">
        <el-menu-item v-for="c in contractTypes" :key="c.key" :index="c.key">
          <span>{{c.label}}</span>
        </el-menu-item>
      </el-menu>
    </el-aside>
    <el-main>
      <el-header style="text-align: left; font-size: 18px">
        <span>{{selectedApiType}} Contracts</span>
        <span style="float: right">
          <el-checkbox v-model="isExcludeExpired" @change="handleIncludedContractsChange">Exclude Expired Contracts</el-checkbox>
        </span>
      </el-header>
      <el-card class="box-card" v-for="c in contracts" :key="c.address">
        <div slot="header" class="clearfix grid-content cell-bold">
          <el-row :gutter="10"><el-col :span="5">Name</el-col><el-col :span="19">{{ c.name }}</el-col></el-row>
        </div>
        <div class="grid-content">
          <el-row :gutter="10"><el-col :span="5">Contract Address</el-col><el-col :span="19">{{ c.address }}</el-col></el-row>
          <el-row :gutter="10"><el-col :span="5">Publisher</el-col><el-col :span="19">{{ c.ownerAddress }}</el-col></el-row>
          <el-row :gutter="10"><el-col :span="5">Description</el-col><el-col :span="19">{{ c.description }}</el-col></el-row>
          <el-row :gutter="10"><el-col :span="5">Contract Type</el-col><el-col :span="19">{{ c.type }}</el-col></el-row>
          <el-row :gutter="10"><el-col :span="5">Expiration Date</el-col><el-col :span="19">{{ c.expirationTime }}</el-col></el-row>
          <el-row :gutter="10"><el-col :span="5">Location</el-col><el-col :span="19">{{ c.location }}</el-col></el-row>
          <el-row :gutter="10"><el-col :span="5">Contract Strike</el-col><el-col :span="19">{{ c.condition }}</el-col></el-row>
          <el-row :gutter="10"><el-col :span="5">Min Premium (ETH)</el-col><el-col :span="19">{{ c.minimumPremium }}</el-col></el-row>
          <el-row :gutter="10"><el-col :span="5">Valuation Date</el-col><el-col :span="19">{{ c.valuationTime }}</el-col></el-row>
          <el-row :gutter="10"><el-col :span="5">Balance (ETH)</el-col><el-col :span="19">{{ c.balance }}</el-col></el-row>
          <el-row :gutter="10"><el-col :span="5">Forecast</el-col><el-col :span="19">{{ c.forecast }}</el-col></el-row>
          <el-row :gutter="10"><el-col :span="5">Forecast Risk</el-col><el-col :span="19">{{ c.forecastRisk }}</el-col></el-row>
          <el-row :gutter="10">
            <el-col :span="5">Source Code</el-col><el-col :span="7"><el-button :disabled="!c.hasCompiledCode" type="primary" size="mini" icon="el-icon-view" round @click="showSol(c.contractFileId)"></el-button></el-col>
            <el-col :span="5">Abi</el-col><el-col :span="7"><el-button :disabled="!c.hasSourceCode" type="primary" size="mini" icon="el-icon-view" round @click="showJson(c.contractFileId)"></el-button></el-col>
          </el-row>
        </div>
      </el-card>
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
    </el-main>
  </el-container>
</template>

<style scoped>
  .el-header {
    background-color: #B3C0D1;
    color: #333;
    line-height: 60px;
  }
  .el-aside {
    text-align: left;
    margin: -20px 0px 0px -20px;
  }
  .el-main {
    text-align: left;
    padding: 0px 0px 20px 0px;
    margin: 0px 0px 0px 20px;
  }
  .el-row {
    margin-bottom: 20px;
  }
  .el-col {
    border-radius: 4px;
  }
  .bg-purple-dark {
    background: #99a9bf;
  }
  .bg-purple {
    background: #d3dce6;
  }
  .bg-purple-light {
    background: #e5e9f2;
  }
  .grid-content {
    border-radius: 4px;
    min-height: 36px;
    padding-left: 20px;
  }
  .row-bg {
    padding: 10px 0;
    background-color: #f9fafc;
  }
  .cell-bold {
    margin-top: 20px;
    margin-bottom: -10px;
    font-weight: bold;
  }
  .clearfix:before,
  .clearfix:after {
    display: table;
    content: "";
  }
  .clearfix:after {
    clear: both
  }
  .box-card {
    display: block;
    float: left;
    width: 750px;
    margin-top: 20px;
    margin-right: 20px;
    font-size: small;
  }
</style>

<script>

export default {
  props: ['prod'],
  data () {
    return {
      contractTypes: [{label: 'Weather Insurance', key: 'WeatherInsurance'}],
      selectedApiType: 'WeatherInsurance',
      isExcludeExpired: true,
      contracts: [],
      showContractFileContent: false,
      contractFileContentTitle: '',
      contractFileContent: ''
    }
  },
  mounted: function () {
    this.prod.getRegisteredWeatherInsuranceContractsForApiType(this.selectedApiType, this.isExcludeExpired).then(contracts => {
      this.contracts = contracts
    })
  },
  methods: {
    async contractTypeChanged (key, keyPath) {
      this.selectedApiType = key
      this.contracts = await this.prod.getRegisteredWeatherInsuranceContractsForApiType(this.selectedApiType, this.isExcludeExpired)
    },
    async handleIncludedContractsChange () {
      this.contracts = await this.prod.getRegisteredWeatherInsuranceContractsForApiType(this.selectedApiType, this.isExcludeExpired)
    },
    async showJson (fileId) {
      const selectedContractFile = await this.prod.getContractFile(fileId, true, false)
      this.contractFileContentTitle = 'Abi and Bytecode'
      this.contractFileContent = `Abi: ${JSON.stringify(selectedContractFile.abi)}\r\n\r\nBytecode: ${selectedContractFile.bytecode}`
      this.showContractFileContent = true
    },
    async showSol (fileId) {
      const selectedContractFile = await this.prod.getContractFile(fileId, false, true)
      this.contractFileContentTitle = 'Source Code'
      this.contractFileContent = `${selectedContractFile.sourceCode}`
      this.showContractFileContent = true
    },
    yesNo (val) {
      if (val) {
        return 'yes'
      } else {
        return 'no'
      }
    }
  }
}
</script>
