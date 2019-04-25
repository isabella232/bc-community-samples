<template>
  <el-container>
    <el-aside width="100px">
      <a href="https://github.com/stejin/bc-community-samples/tree/master/weather-insurance-app/stejin/"><img class="logo" src="../assets/D-Blue.png"/></a>
    </el-aside>
    <el-main>
      <el-menu :default-active="$router.currentRoute.path" class="el-menu" mode="horizontal" :router="true" text-color="#409EFF" active-text-color="#409EFF" >
        <el-menu-item index="/eth">
          <v-icon name="brands/ethereum"></v-icon>
          <span>ETH</span>
          <el-tag>{{network}}</el-tag>
        </el-menu-item>
        <!--el-menu-item index="/">About</el-menu-item>
        <el-menu-item index="/hello">Hello Vue</el-menu-item-->
      </el-menu>
    </el-main>
    <el-row>
      <site-services />
    </el-row>
  </el-container>
</template>

<script>
import SiteServices from '../components/SiteServices'
import { serverBus } from '../main'

export default {
  components: {
    SiteServices
  },
  data: function () {
    return {
      network: 'Unknown'
    }
  },
  created () {
    // Using the server bus
    serverBus.$on('networkDetected', (network) => {
      this.network = network
    })
  }
}
</script>

<style scoped>
  .logo {
    margin-left: 10px;
    margin-top: 10px;
    width: 50px;
  }
  .el-menu {
    border-bottom: 1px transparent;
  }
  .el-menu--horizontal .el-menu-item {
    border-bottom: 2px transparent;
    border-top: 2px solid transparent;
    border-top-left-radius: 20px;
    border-top-right-radius: 20px;
  }
  .el-menu-item {
    height: 45px;
    line-height: 45px;
    font-size: 18px;
    font-weight: bold;
  }
  .el-menu-item.is-active {
    border-bottom: transparent;
    border-top: 2px solid #409EFF;
  }
  .el-row {
    margin-top: 20px;
  }
</style>


<!--"#409EFF"-->
