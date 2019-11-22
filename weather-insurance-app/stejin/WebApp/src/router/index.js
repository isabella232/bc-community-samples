import Vue from 'vue'
import Router from 'vue-router'
import Eth from '../components/Eth.vue'
import SiteServices from '../components/SiteServices'
import Splash from '../components/Splash.vue'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'splash',
      component: Splash
    },
    {
      path: '/eth',
      name: 'ETH',
      component: Eth
    },
    {
      path: '/service',
      name: 'service',
      component: SiteServices
    }
  ]
})