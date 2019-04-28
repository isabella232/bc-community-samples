<template>
  <el-container>
      <el-button type="primary" icon="el-icon-key" disabled @click="notimplemented">Identity</el-button>
      <el-button type="primary" icon="el-icon-service" disabled @click="notimplemented">Support</el-button>
      <!--el-button type="primary" @click="api">GET API</el-button>
      <el-button type="primary" @click="api1">POST API</el-button>
      <el-button type="primary" @click="logout">Logout {{loggedInAccountAddress}}</el-button-->
  </el-container>
</template>

<script>
export default {
  methods: { 
    notimplemented () {
    }
  }
}
</script>
