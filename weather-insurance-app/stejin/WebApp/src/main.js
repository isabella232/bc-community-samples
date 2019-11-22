import Vue from 'vue'
import Resource from 'vue-resource'
import ElementUI from 'element-ui'
import 'element-ui/lib/theme-chalk/index.css'
import App from './App.vue'
import router from './router'
import lang from 'element-ui/lib/locale/lang/en'
import locale from 'element-ui/lib/locale'
import 'vue-awesome/icons/brands/ethereum'
import Icon from 'vue-awesome/components/Icon'
import 'normalize.css'
import 'element-ui/lib/theme-chalk/reset.css'

export const serverBus = new Vue()

// configure language
locale.use(lang)

Vue.use(ElementUI)
Vue.use(Resource)

Vue.component('v-icon', Icon)

new Vue({
  el: '#app',
  router,
  components: { App },
  template: '<App/>',
  render: h => h(App)
})
