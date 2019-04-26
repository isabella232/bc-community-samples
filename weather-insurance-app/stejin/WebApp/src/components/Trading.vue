<template>
  <el-container style="margin-bottom: 0px; padding-bottom: 0px;">
    <el-aside width="200px" style="margin-top: -20px; margin-left: -20px;">
      <el-menu @select="handleSelect">
        <el-menu-item v-for="c in contracts" :key="c.name" :index="c.address">
          <span>{{c.name}}</span>
        </el-menu-item>
      </el-menu>
    </el-aside>
    <el-main v-if="selectedContract" style="margin-top: -20px; margin-bottom: 0px; padding-bottom: 0px;">
      <el-row>
        <el-col :span="18"><trades :prod=prod :contract=selectedContract /></el-col>
        <el-col :span="6"><orders :prod=prod :contract=selectedContract /></el-col>
      </el-row>
    </el-main>
  </el-container>
</template>

<script>
import Trades from './Trades'
import Orders from './Orders'

export default {
  props: ['prod'],
  components: { Trades, Orders },
  data () {
    return {
      contracts: [],
      selectedContract: null
    }
  },
  mounted: function () {
    this.prod.getRegisteredWeatherInsuranceContractsForApiType('WeatherInsurance', true).then(contracts => {
      this.contracts = contracts.sort((a, b) => a.name.localeCompare(b.name))
    })
  },
  methods: {
    handleSelect (contractAddress) {
      this.selectedContract = this.contracts.filter(d => d.address === contractAddress)[0]
    }
  }
}
</script>
