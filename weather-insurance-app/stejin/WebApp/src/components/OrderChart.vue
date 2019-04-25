<script>
import { HorizontalBar } from 'vue-chartjs'

export default {
  extends: HorizontalBar,
  props: ['chartData', 'height', 'width'],
  data () {
    return {
      options: {
        legend: {
          display: false
        },
        scales: {
          xAxes: [{
            stacked: true
          }],
          yAxes: [{
            stacked: true
          }]
        }
      }
    }
  },
  watch: {
    chartData: function (val) {
      this.renderChart(this.chartData, this.options)
      // this.$data._chart.update()
    }
  },
  mounted () {
    this.renderChart(this.chartData, this.options)
  }
}
</script>
