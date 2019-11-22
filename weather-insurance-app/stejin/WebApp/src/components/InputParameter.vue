<template>
  <el-form-item :label="calcLabel">
    <el-input :placeholder="input.type" v-model="value" @change="onChange"></el-input>
  </el-form-item>
</template>

<style>
  .text {
    font-size: 14px;
  }

  .item {
    margin-bottom: 18px;
  }
</style>

<script>
export default {
  props: [ 'input' ],
  data () {
    return {
      value: null
    }
  },
  computed: {
    calcLabel: function () {
      if (!this.input.name) {
        return this.input.type === 'address' ? 'address' : 'index'
      }
      return this.input.name
    }
  },
  methods: {
    onChange (val) {
      this.$emit('onChange', this.input.name, val)
    }
  }
}
</script>
