<template>
  <el-container style="margin: -20px; margin-bottom: 0px; padding-bottom: 0px;">
    <el-header height="500px">
      <div class="chartcontainer" v-loading="loading">
        <div class="chartdiv" ref="chartdiv" />
      </div>
    </el-header>
    <el-main style="margin-bottom: 0px; padding-bottom: 0px;">
      <el-table
        ref="tradeTable"
        v-loading="loading"
        :data="sortedTrades"
        stripe
        height="330"
        style="width: 100%; font-size: small;">
        <el-table-column
          prop="timestamp"
          label="Timestamp"
          width="170">
        </el-table-column>
        <el-table-column
          label="Change"
          width="70">
          <template slot-scope="scope">
            <i :class="getPriceChangeIcon(scope.row.priceChange)"></i>
          </template>
        </el-table-column>
        <el-table-column
          prop="notional"
          label="Notional"
          width="110">
        </el-table-column>
        <el-table-column
          prop="premium"
          label="Total Premium"
          width="170">
        </el-table-column>
        <el-table-column
          prop="price"
          label="Price"
          width="170">
        </el-table-column>
        <el-table-column
          prop="user"
          label="User Address"
          width="340">
        </el-table-column>
      </el-table>
    </el-main>
  </el-container>
</template>

<style scoped>
.chartcontainer {
  width: 100%;
  height: 500px;
  position: relative;
}
.chartdiv, .curtain {
  width: 100%;
  height: 500px;
  font-size: small;
  position: absolute;
}
</style>

<script>
import * as am4core from '@amcharts/amcharts4/core'
import * as am4charts from '@amcharts/amcharts4/charts'

// eslint-disable-next-line
import am4themes_frozen from '@amcharts/amcharts4/themes/frozen'
// eslint-disable-next-line
import am4themes_animated from '@amcharts/amcharts4/themes/animated'

am4core.useTheme(am4themes_frozen)
am4core.useTheme(am4themes_animated)

export default {
  props: ['prod', 'contract'],
  data () {
    return {
      loading: false,
      trades: [],
      listener: null
    }
  },
  created: async function () {
    await this.init()
  },
  computed: {
    sortedTrades: function () {
      const result = []
      for (const i in this.trades) {
        const t = this.trades[i]
        const r = {
          ts: t.timestamp,
          timestamp: t.timestamp.toLocaleString(),
          priceChange: t.priceChange,
          notional: t.notional,
          premium: t.premium,
          price: t.price,
          user: t.user
        }
        result.push(r)
      }
      result.sort((a, b) => a.ts < b.ts ? 1 : -1)
      return result
    }
  },
  async beforeDestroy () {
    if (this.listener) {
      await this.prod.stopWatch(this.listener)
    }
    if (this.$refs.chartdiv) {
      this.$refs.chartdiv.dispose()
    }
  },
  watch: {
    contract: async function (val) {
      await this.init()
    }
  },
  methods: {
    async init () {
      this.trades = await this.getTrades()
      this.renderChart()
      if (this.listener) {
        await this.prod.stopWatch(this.listener)
      }
      this.listener = this.prod.startListeningForInsuranceBoughtEvents(this.contract.address, this.addTrade)
    },
    async getTrades () {
      this.loading = true
      const trades = await this.prod.get(`purchases/${this.contract.address}`)
      const result = []
      for (const i in trades) {
        const t = trades[i]
        const trade = {
          timestamp: new Date(t.timestamp),
          notional: t.notional,
          premium: this.round(t.premium),
          price: this.round(t.premium / t.notional),
          user: t.userAddress
        }
        result.push(trade)
      }
      let lastPrice = 0
      for (const t in result.sort((a, b) => a.timestamp > b.timestamp ? 1 : -1)) {
        const trade = result[t]
        if (lastPrice === 0) { // First trade
          trade.priceChange = 'unchanged'
        } else {
          trade.priceChange = this.getPriceChange(lastPrice, trade.price)
        }
        lastPrice = trade.price
      }
      this.loading = false
      return result // trades.sort((a, b) => a > b ? 1 : -1)
    },
    addTrade (newTrade) {
      const now = new Date()
      const nowUtc = new Date(now.getTime() + (now.getTimezoneOffset() * 60000))
      const trade = {
        timestamp: nowUtc,
        notional: this.prod.convertToAccountingUnit(newTrade.notional),
        premium: this.round(this.prod.convertToAccountingUnit(newTrade.premium)),
        price: this.round(newTrade.premium / newTrade.notional),
        user: newTrade.user
      }
      if (this.trades.length === 0) {
        trade.priceChange = 'unchanged'
      } else {
        trade.priceChange = this.getPriceChange(this.trades[this.trades.length - 1].price, trade.price)
      }
      this.trades.push(trade)
      this.renderChart()
    },
    round (val) {
      return Math.round(10000 * val) / 10000
    },
    getPriceChange (oldPrice, newPrice) {
      if (newPrice > oldPrice) {
        return 'up'
      }
      if (newPrice < oldPrice) {
        return 'down'
      }
      if (newPrice === oldPrice) {
        return 'unchanged'
      }
    },
    getPriceChangeNumber (priceChange) {
      if (priceChange === 'up') {
        return 1
      }
      if (priceChange === 'down') {
        return -1
      }
      if (priceChange === 'unchanged') {
        return 0
      }
    },
    getPriceChangeIcon (priceChange) {
      if (priceChange === 'up') {
        return 'el-icon-caret-top'
      }
      if (priceChange === 'down') {
        return 'el-icon-caret-bottom'
      } else {
        return 'el-icon-d-caret'
      }
    },
    renderChart () {
      const rawdata = this.trades
      const data = []
      if (rawdata.length > 1) {
        for (let i = 1; i < rawdata.length; i++) {
          data.push({
            'timestamp': rawdata[i].timestamp,
            'price': rawdata[i].price,
            'volume': rawdata[i].notional,
            'change': this.getPriceChangeNumber(rawdata[i].priceChange)
          })
        }
      }

      // console.log(data)

      let chart = am4core.create('chartdiv', am4charts.XYChart)

      let interfaceColors = new am4core.InterfaceColorSet()

      chart.data = data
      // the following line makes value axes to be arranged vertically.
      chart.leftAxesContainer.layout = 'vertical'

      // uncomment this line if you want to change order of axes
      // chart.bottomAxesContainer.reverseOrder = true

      let dateAxis = chart.xAxes.push(new am4charts.DateAxis())
      dateAxis.renderer.grid.template.location = 0
      dateAxis.renderer.ticks.template.length = 8
      dateAxis.renderer.ticks.template.strokeOpacity = 0.1
      dateAxis.renderer.grid.template.disabled = true
      dateAxis.renderer.ticks.template.disabled = false
      dateAxis.renderer.ticks.template.strokeOpacity = 0.2

      // these two lines makes the axis to be initially zoomed-in
      // dateAxis.start = 0.7
      // dateAxis.keepSelection = true

      let valueAxis = chart.yAxes.push(new am4charts.ValueAxis())
      valueAxis.tooltip.disabled = true
      valueAxis.zIndex = 2
      valueAxis.renderer.baseGrid.disabled = true
      valueAxis.renderer.inside = true
      // height of axis
      valueAxis.height = am4core.percent(80)
      valueAxis.renderer.labels.template.verticalCenter = 'bottom'
      valueAxis.renderer.labels.template.padding(2, 2, 2, 2)
      // valueAxis.renderer.maxLabelPosition = 0.95
      valueAxis.renderer.fontSize = '0.8em'

      // uncomment these lines to fill plot area of this axis with some color
      valueAxis.renderer.gridContainer.background.fill = interfaceColors.getFor('alternativeBackground')
      valueAxis.renderer.gridContainer.background.fillOpacity = 0.05

      let series = chart.series.push(new am4charts.LineSeries())
      series.dataFields.dateX = 'timestamp'
      series.dataFields.valueY = 'price'
      series.tooltipText = '{valueY.value}'
      series.name = 'Price'
      series.yAxis = valueAxis
      series.strokeWidth = 1
      series.stroke = am4core.color('#409EFF')

      let valueAxis2 = chart.yAxes.push(new am4charts.ValueAxis())
      valueAxis2.tooltip.disabled = true
      valueAxis2.renderer.inside = true
      // height of axis
      valueAxis2.height = am4core.percent(20)
      valueAxis2.zIndex = 1
      // this makes gap between panels
      valueAxis2.marginTop = 20
      valueAxis2.renderer.baseGrid.disabled = true
      valueAxis2.renderer.labels.template.verticalCenter = 'bottom'
      valueAxis2.renderer.labels.template.padding(2, 2, 2, 2)
      // valueAxis2.renderer.maxLabelPosition = 0.95
      valueAxis2.renderer.fontSize = '0.8em'

      // uncomment these lines to fill plot area of this axis with some color
      valueAxis2.renderer.gridContainer.background.fill = interfaceColors.getFor('alternativeBackground')
      valueAxis2.renderer.gridContainer.background.fillOpacity = 0.05

      let series2 = chart.series.push(new am4charts.ColumnSeries())
      series2.dataFields.dateX = 'timestamp'
      series2.dataFields.valueY = 'volume'
      series2.dataFields.valueX = 'change'
      series2.yAxis = valueAxis2
      series2.tooltipText = '{valueY.value}'
      series2.name = 'Volume'
      series2.strokeWidth = 2

      series2.heatRules.push({
        'target': series2.columns.template,
        'property': 'stroke',
        'min': am4core.color('#dd6161'),
        'max': am4core.color('#5daf34'),
        'dataField': 'valueX'
      })

      series2.heatRules.push({
        'target': series2.columns.template,
        'property': 'fill',
        'min': am4core.color('#dd6161'),
        'max': am4core.color('#5daf34'),
        'dataField': 'valueX'
      })

      chart.cursor = new am4charts.XYCursor()
      chart.cursor.xAxis = dateAxis

      let scrollbarX = new am4charts.XYChartScrollbar()
      scrollbarX.series.push(series)
      scrollbarX.marginBottom = 20
      chart.scrollbarX = scrollbarX
    }
  }
}
</script>
