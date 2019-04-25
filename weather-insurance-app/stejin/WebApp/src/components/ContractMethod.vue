<template>
  <el-card shadow="hover" :class="cardClass">
    <div slot="header" class="clearfix">
      <span>{{ method.name }}</span>
      <el-button style="float: right; padding: 3px 0" type="text" @click="execute">Execute</el-button>
    </div>
    <el-form ref="form" :model="method" :label-width="labelWidth">
      <input-parameter v-for="parameter in parameters" :key="parameter.name" :input="parameter" @onChange="onChange" />
    </el-form>
    <div v-if="result">
      <el-collapse accordion>
        <el-collapse-item name="1">
          <template slot="title">
            <i class="header-icon el-icon-info" style="padding-right: 3px"></i> Result
          </template>
          <div>{{ result }}</div>
        </el-collapse-item>
      </el-collapse>
    </div>
  </el-card>
</template>

<style>
  .clearfix:before,
  .clearfix:after {
    display: table;
    content: "";
  }
  .clearfix:after {
    clear: both
  }
  .box-card-constructor {
    display: block;
    float: left;
    width: 580px;
    margin-bottom: 20px;
    margin-right: 20px;
  }
  .box-card-function {
    display: block;
    float: left;
    width: 480px;
    margin-bottom: 20px;
    margin-right: 20px;
  }
</style>

<script>
import InputParameter from '../components/InputParameter'
import { serverBus } from '../main'

export default {
  components: { InputParameter },
  props: [ 'prod', 'owner', 'contractFile', 'contractAddress', 'method' ],
  data () {
    return {
      values: {},
      result: null
    }
  },
  created () {
    // Using the server bus
    serverBus.$on('contractChanged', () => {
      this.result = null
    })
  },
  computed: {
    parameters: function () {
      return this.method['inputs']
    },
    cardClass: function () {
      if (this.method.type === 'constructor') {
        return 'box-card-constructor'
      } else {
        return 'box-card-function'
      }
    },
    labelWidth: function () {
      if (this.method.type === 'constructor') {
        return '180px'
      } else {
        return '120px'
      }
    }
  },
  methods: {
    onChange (name, val) {
      this.values[name] = val
    },
    async execute () {
      let parameters = []
      for (const i in this.method.inputs) {
        const input = this.method.inputs[i]
        parameters.push(this.values[input.name])
      }

      if (this.method.type === 'constructor') {
        this.$emit('onConstructorExecuted', parameters)
      }

      if (this.method.type === 'function') {
        const resultsArray = []
        const results = await this.prod.executeContractFunction(this.owner, this.contractAddress, this.contractFile.abi, this.method, parameters)
        const outputs = this.method.outputs
        resultsArray.push(results)
        for (const o in outputs) {
          const result = results[o]
          const output = outputs[o]
          resultsArray.push({type: output.type, value: output.type.startsWith('int') || output.type.startsWith('uint') ? parseInt(result) : result})
        }
        this.result = resultsArray
        this.$emit('onFunctionExecuted', this.method.name)
      }
    }
  }
}
</script>
