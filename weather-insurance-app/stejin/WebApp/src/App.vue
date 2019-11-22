<template>
  <div id="app">
    <el-container :style="style">
      <el-header style="height: 60px">
        <main-menu />
      </el-header>
      <el-main style="height: 100%">
        <router-view />
      </el-main>
    </el-container>
  </div>
</template>

<script>
import MainMenu from './components/MainMenu'

export default {
  components: {
    MainMenu
  },
  name: 'app',
  data () {
    return {
      windowHeight: window.innerHeight
    }
  },
  computed: {
    style: function () {
      return `height: ${this.windowHeight}px`
    }
  },
  mounted: function () {
    let that = this
    this.$nextTick(function () {
      window.addEventListener('resize', function (e) {
        that.windowHeight = window.innerHeight
      })
    })
  }
}
</script>

<style>
#app {
  height: 100%;
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}
</style>
