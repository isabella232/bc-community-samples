<template>
  <el-container style="height: 100%">
    <el-aside width="140px">
      <el-menu :default-active="activeIndex" @select="handleSelect">
        <el-menu-item index="1">
          <i class="el-icon-goods"></i>
          <span>Buy Insurance</span>
        </el-menu-item>
        <el-menu-item index="2">
          <i class="el-icon-document"></i>
          <span>Contracts</span>
        </el-menu-item>
        <el-menu-item index="3">
          <i class="el-icon-setting"></i>
          <span>Operations</span>
        </el-menu-item>
      </el-menu>
    </el-aside>
    <el-main class="el-container-prod">
      <component :is="currentView" :prod="prod"/>
    </el-main>
  </el-container>
</template>

<style scoped>
  .el-aside {
    margin-top: 1px;
    margin-left: -20px;
  }
  .el-menu {
    margin-left: -10px;
    text-align: left;
    border-right: transparent 0px;
  }
  .el-container-prod {
      border-top: solid 1px #e6e6e6;
      border-left: solid 1px #e6e6e6;
  }
</style>

<script>
import prodeth from '../modules/prodeth'
import NetworkError from '../components/NetworkError'
import Operations from '../components/Operations'
import Contracts from '../components/Contracts'
import Trading from '../components/Trading'
import { serverBus } from '../main'

export default {
  data () {
    return {
      prod: new prodeth.ProD(this.$http),
      activeIndex: '1',
      currentView: NetworkError,
      isInitialized: false
    }
  },
  prod: this.prod,
  mounted: async function () {
    if (typeof this.prod.eth !== 'undefined') {
      await this.prod.initialize()
      serverBus.$emit('networkDetected', this.prod.network.name.split(':')[1])
      this.isInitialized = true
      this.currentView = Trading
    } else {
      this.currentView = NetworkError
    }
  },
  methods: {
    handleSelect (key, keyPath) {
      if (this.isInitialized) {
        switch (key) {
          case '1':
            this.currentView = Trading
            break
          case '2':
            this.currentView = Contracts
            break
          case '3':
            this.currentView = Operations
            break
        }
      } else {
        this.currentView = NetworkError
      }
    }
  }
}
</script>
