<template>
  <el-container>
    <el-header height="430px">
      <order-chart
        ref="premiumChart"
        :chartData="chartPremiums"
        height="100px"
        width="100px"/>
      <br/>
      <el-row :gutter="10">
        <el-col :span="8"><div class="contract-info">Valuation Time</div></el-col>
        <el-col :span="16"><div class="contract-info bg-blue">{{valuationTime}}</div></el-col>
      </el-row>
      <el-row :gutter="10">
        <el-col :span="8"><div class="contract-info">Condition Strike</div></el-col>
        <el-col :span="16"><div class="contract-info bg-blue">{{strike}}</div></el-col>
      </el-row>
      <el-row :gutter="10">
        <el-col :span="8"><div class="contract-info">Last Forecast</div></el-col>
        <el-col :span="16"><div class="contract-info bg-blue">{{forecast}}</div></el-col>
      </el-row>
      <el-row :gutter="10">
        <el-col :span="8"><div class="contract-info">Forecast Risk</div></el-col>
        <el-col :span="16"><div class="contract-info bg-blue">{{forecastRisk}}</div></el-col>
      </el-row>
    </el-header>
    <el-main>
      <el-row :gutter="20" class="frm-row">
        <el-select v-model="selectedUserAccountAddress" placeholder="Select Address" size="small" style="width: 350px" @change="handleUserAccountChange">
          <el-option
            v-for="userAccount in userAccounts"
            :key="userAccount.address"
            :label="userAccount.address"
            :value="userAccount.address">
          </el-option>
        </el-select>
      </el-row>
      <div v-if="position">
        <el-row :gutter="5" class="frm-row">
          <el-col :span="8"><div class="frm-label">Balance</div></el-col>
          <el-col :span="16"><div class="order-info">{{balance}}</div></el-col>
        </el-row>
        <el-row :gutter="5" class="frm-row">
          <el-col :span="8"><div class="frm-label">Notional Bought</div></el-col>
          <el-col :span="16"><div class="order-info">{{position.notional}}</div></el-col>
        </el-row>
        <el-row :gutter="5" class="frm-row">
          <el-col :span="8"><div class="frm-label">Premium Paid</div></el-col>
          <el-col :span="16"><div class="order-info">{{position.premium}}</div></el-col>
        </el-row>
        <el-row :gutter="5" class="frm-row">
          <el-col :span="8"><div class="frm-label">Notional</div></el-col>
          <el-col :span="16"><el-input-number v-model="notional" size="medium" :min="1" :controls="true"></el-input-number></el-col>
        </el-row>
        <el-row :gutter="5" class="frm-row">
          <el-col :span="8"><div class="frm-label">Total Premium</div></el-col>
          <el-col :span="16"><div class="order-info">{{premium}}</div></el-col>
        </el-row>
        <el-row :gutter="5" class="frm-row" type="flex" justify="center">
          <el-col :span="24"><el-button type="success" round @click="buyInsurance">Buy</el-button></el-col>
        </el-row>
      </div>
    </el-main>
  </el-container>
</template>

<style scoped>
  .el-row {
    margin-bottom: 10px;
  }
  .el-row:last-child {
    margin-bottom: 0;
  }
  .frm-row {
    padding-top: 20px;
  }
  .frm-label {
    font-size: small;
    text-align: right;
  }
  .contract-info {
    text-align: left;
    font-size: small;
  }
  .order-info {
    font-size: small;
  }
  .bg-red {
    color: #dd6161;
  }
  .bg-purple {
    background: #d3dce6;
    color: #714D9F;
  }
  .bg-green {
    color: #5daf34;
  }
  .bg-blue {
    color: #409EFF;
  }
</style>

<script>
import OrderChart from './OrderChart'

export default {
  props: ['prod', 'contract'],
  components: {
    OrderChart
  },
  data () {
    return {
      listener: null,
      userAccounts: [],
      selectedUserAccountAddress: '',
      balance: '',
      loading: false,
      premiums: [],
      chartPremiums: null,
      orderListener: null,
      tradeListener: null,
      position: null,
      notional: 1.0,
      premium: 0.0,
      valuationTime: null,
      strike: null,
      forecast: null,
      forecastRisk: null
    }
  },
  created: async function () {
    if (this.listener) {
      await this.prod.stopWatch(this.listener)
    }
    this.listener = this.prod.startBlockWatch(() => {
      this.updateDetails()
    })
  },
  beforeDestroy: async function () {
    if (this.listener) {
      await this.prod.stopWatch(this.listener)
    }
    if (this.$refs.premiumChart) {
      this.$refs.premiumChart.dispose()
    }
  },
  mounted: function () {
    this.prod.getUserAccounts().then(result => {
      this.userAccounts = result
    })
    this.init()
  },
  watch: {
    contract: async function (val) {
      await this.init()
    },
    notional: async function () {
      await this.updatePremium()
    }
  },
  methods: {
    async init () {
      await this.updatePremium()
      await this.updateDetails()
    },
    async updateDetails () {
      const d = await this.prod.getPublicDeployedWeatherInsuranceContractDetails(this.contract, this.selectedUserAccountAddress)
      if (this.selectedUserAccountAddress) {
        const b = await this.prod.getBalance(this.selectedUserAccountAddress)
        this.balance = `${this.prod.convertToAccountingUnit(b)} ETH`
        this.position = {
          notional: d.notional,
          premium: d.premium 
        }
      }
      this.valuationTime = d.valuationTime
      this.strike = d.condition
      this.forecast = d.forecast
      this.forecastRisk = d.forecastRisk
      this.premiums = await this.getPremiums()
      this.constructChartData()
    },
    async handleUserAccountChange (val) {
      this.position = null
      this.selectedUserAccountAddress = val
      await this.updateDetails()
    },
    async updatePremium () {
      const p = await this.prod.getPremium(this.contract.address, this.prod.convertToBaseUnit(this.notional))
      this.premium = this.prod.convertToAccountingUnit(p)
    },
    async getPremiums () {
      const result = []
      const steps = [5, 10]
      for (let i = 0; i < 4; i++) {
        for (const j in steps) {
          const notional = 0.1 * steps[j] * 10 ** i
          const premium = await this.prod.getPremium(this.contract.address, this.prod.convertToBaseUnit(notional))
          const r = {
            notional: notional,
            premium: this.prod.convertToAccountingUnit(premium)
          }
          result.push(r)
        }
      }
      return result
    },
    constructChartData () {
      this.chartPremiums = this.getChartPremiums(this.premiums.sort((a, b) => b.notional - a.notional))
    },
    getChartPremiums (data) {
      const gradient = this.$refs.premiumChart.$refs.canvas.getContext('2d').createLinearGradient(0, 0, 350, 0)
      gradient.addColorStop(0, 'rgba(0,255,0,0.8)')
      gradient.addColorStop(0.5, 'rgba(255, 255, 0, 0.8)')
      gradient.addColorStop(1, 'rgba(255,0,0,0.8)')

      return {
        labels: data.map(d => d.notional),
        datasets: [{
          //backgroundColor: 'rgba(0,255,0,0.8)',
          backgroundColor: gradient,
          pointBackgroundColor: 'rgba(179,181,198,1)',
          pointBorderColor: '#fff',
          pointHoverBackgroundColor: '#fff',
          pointHoverBorderColor: 'rgba(179,181,198,1)',
          data: data.map(d => d.premium)
        }]
      }
    },
    async buyInsurance () {
      this.prod.buyInsurance(this.selectedUserAccountAddress, this.contract.address, this.prod.convertToBaseUnit(this.notional), this.prod.convertToBaseUnit(this.premium)).then(res => {
        if (res.status) {
          this.$message({
            type: 'success',
            message: 'Buy order placed'
          })
        } else {
          this.$message({
            type: 'error',
            showClose: true,
            duration: 0,
            message: `Error while attempting to place buy order. ${res.transactionHash}`
          })
        }
      })
    }
  }
}
</script>
