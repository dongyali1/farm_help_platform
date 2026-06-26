<template>
  <el-container class="app-container">
    <el-header class="app-header">
      <div class="logo">🌾 助农平台</div>
      <el-menu :default-active="activeMenu" mode="horizontal" class="header-menu">
        <el-menu-item index="home" @click="activeMenu = 'home'">首页</el-menu-item>
        <el-menu-item index="products" @click="activeMenu = 'products'">商品列表</el-menu-item>
        <el-menu-item index="farmers" @click="activeMenu = 'farmers'">入驻农户</el-menu-item>
        <el-menu-item index="purchases" @click="handlePurchasesClick">需求广场</el-menu-item>
        <el-menu-item index="reviews" @click="handleReviewsClick">用户评价</el-menu-item>
        <el-menu-item index="farmer-product" v-if="currentUser?.role === 'farmer'" @click="activeMenu = 'farmer-product'">商品管理</el-menu-item>
        <el-menu-item index="admin" v-if="currentUser?.role === 'admin'" @click="activeMenu = 'admin'">管理后台</el-menu-item>
      </el-menu>
      <div class="user-info">
        <el-button v-if="!currentUser" @click="showLogin = true">登录</el-button>
        <el-dropdown v-else trigger="click">
          <template #default>
            <el-button type="text" style="color: white;">
              {{ currentUser.username }}
              <i class="el-icon-arrow-down el-icon--right"></i>
            </el-button>
          </template>
          <template #dropdown>
            <el-dropdown-item @click.native="handleSwitchAccount">切换账号</el-dropdown-item>
            <el-dropdown-item @click.native="logout" divided>退出登录</el-dropdown-item>
          </template>
        </el-dropdown>
      </div>
    </el-header>

    <el-main>
      <Home v-if="activeMenu === 'home'" @navigate="handleNavigate" />
      <ProductList v-else-if="activeMenu === 'products'" :currentUser="currentUser" @showLogin="showLogin = true" />
      <FarmerList v-else-if="activeMenu === 'farmers'" />
      <template v-else-if="activeMenu === 'purchases'">
        <PurchaseList v-if="currentUser" :currentUser="currentUser" />
        <div v-else class="login-required">
          <el-alert title="请先登录" type="warning" show-icon />
          <el-button type="primary" @click="showLogin = true">立即登录</el-button>
        </div>
      </template>
      <template v-else-if="activeMenu === 'reviews'">
        <ReviewList v-if="currentUser" :key="activeMenu + Date.now()" />
        <div v-else class="login-required">
          <el-alert title="请先登录" type="warning" show-icon />
          <el-button type="primary" @click="showLogin = true">立即登录</el-button>
        </div>
      </template>
      <FarmerProduct v-else-if="activeMenu === 'farmer-product'" :current-user="currentUser" />
      <AdminPanel v-else-if="activeMenu === 'admin'" />
    </el-main>

    <!-- 登录弹窗 -->
    <el-dialog title="用户登录" v-model="showLogin" width="400px">
      <el-form :model="loginForm" label-width="80px">
        <el-form-item label="用户名">
          <el-input v-model="loginForm.username" placeholder="请输入用户名" />
        </el-form-item>
        <el-form-item label="密码">
          <el-input v-model="loginForm.password" type="password" placeholder="请输入密码" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleLogin">登录</el-button>
          <el-button @click="showRegister = true">注册</el-button>
        </el-form-item>
      </el-form>
    </el-dialog>

    <!-- 注册弹窗 -->
    <el-dialog title="用户注册" v-model="showRegister" width="400px">
      <el-form :model="registerForm" label-width="80px">
        <el-form-item label="用户名">
          <el-input v-model="registerForm.username" placeholder="请输入用户名" />
        </el-form-item>
        <el-form-item label="密码">
          <el-input v-model="registerForm.password" type="password" placeholder="请输入密码" />
        </el-form-item>
        <el-form-item label="手机号">
          <el-input v-model="registerForm.phone" placeholder="请输入手机号" />
        </el-form-item>
        <el-form-item label="角色">
          <el-select v-model="registerForm.role">
            <el-option label="消费者" value="consumer" />
            <el-option label="农户" value="farmer" />
            <el-option label="管理员" value="admin" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleRegister">注册</el-button>
        </el-form-item>
      </el-form>
    </el-dialog>
  </el-container>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';
import { ElMessage } from 'element-plus';
axios.defaults.baseURL = 'http://localhost:5000/api';
import Home from './components/Home.vue';
import ProductList from './components/ProductList.vue';
import FarmerList from './components/FarmerList.vue';
import PurchaseList from './components/PurchaseList.vue';
import ReviewList from './components/ReviewList.vue';
import FarmerProduct from './components/FarmerProduct.vue';
import AdminPanel from './components/AdminPanel.vue';

const activeMenu = ref('home');
const showLogin = ref(false);
const showRegister = ref(false);
const currentUser = ref(null);
const loginForm = ref({
  username: '',
  password: ''
});
const registerForm = ref({
  username: '',
  password: '',
  phone: '',
  role: 'consumer'
});

const handleLogin = async () => {
  try {
    const response = await axios.post('/login', loginForm.value);
    currentUser.value = response.data.user;
    localStorage.setItem('user', JSON.stringify(currentUser.value));
    showLogin.value = false;
    ElMessage.success('登录成功');
  } catch (error) {
    ElMessage.error(error.response?.data?.error || '登录失败');
  }
};

const handleRegister = async () => {
  try {
    const response = await axios.post('/register', registerForm.value);
    ElMessage.success('注册成功，请登录');
    showRegister.value = false;
  } catch (error) {
    ElMessage.error(error.response?.data?.error || '注册失败');
  }
};

const handlePurchasesClick = () => {
  if (currentUser.value) {
    activeMenu.value = 'purchases';
  } else {
    ElMessage.warning('请先登录');
    showLogin.value = true;
  }
};

const handleReviewsClick = () => {
  if (currentUser.value) {
    activeMenu.value = 'reviews';
  } else {
    ElMessage.warning('请先登录');
    showLogin.value = true;
  }
};

const handleNavigate = (section) => {
  const menuMap = {
    '商品列表': 'products',
    '入驻农户': 'farmers',
    '需求广场': 'purchases',
    '用户评价': 'reviews'
  };
  const menu = menuMap[section] || 'products';
  activeMenu.value = menu;
  ElMessage.success(`已跳转到${section}页面`);
};

const handleSwitchAccount = () => {
  currentUser.value = null;
  localStorage.removeItem('user');
  showLogin.value = true;
  ElMessage.info('请使用其他账号登录');
};

const logout = () => {
  currentUser.value = null;
  localStorage.removeItem('user');
  ElMessage.success('已退出登录');
};

onMounted(() => {
  const user = localStorage.getItem('user');
  if (user) {
    currentUser.value = JSON.parse(user);
  }
});
</script>

<style>
/* ============================================
   助农平台 — 全局基础样式
   ============================================ */

body {
  background-color: var(--bg-page);
  color: var(--text-primary);
  -webkit-font-smoothing: antialiased;
}

/* === 应用容器 === */
.app-container {
  min-height: 100vh;
}

/* === 头部导航 === */
.app-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 24px;
  background: var(--color-header-gradient);
  color: white;
  box-shadow: 0 2px 12px rgba(27, 107, 40, 0.2);
  position: sticky;
  top: 0;
  z-index: 1000;
}

.logo {
  font-size: 26px;
  font-weight: bold;
  letter-spacing: 2px;
  text-shadow: 0 1px 3px rgba(0, 0, 0, 0.15);
}

.header-menu {
  flex: 1;
  margin-left: 50px;
}

.user-info {
  margin-left: auto;
}

.dropdown-trigger {
  cursor: pointer;
  padding: 8px 16px;
  background: rgba(255, 255, 255, 0.15);
  border-radius: 20px;
  color: white;
  font-size: 14px;
  transition: background var(--transition-fast);
}

.dropdown-trigger:hover {
  background: rgba(255, 255, 255, 0.25);
}

/* === 登录要求提示 === */
.login-required {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 400px;
  gap: 20px;
}

/* ============================================
   Element Plus 全局覆盖
   ============================================ */

/* 导航菜单 (header内) */
.header-menu.el-menu--horizontal {
  background: transparent;
  border-bottom: none;
}
.header-menu.el-menu--horizontal .el-menu-item {
  color: rgba(255, 255, 255, 0.85);
  border-bottom: 2px solid transparent;
  transition: all var(--transition-fast);
}
.header-menu.el-menu--horizontal .el-menu-item:hover {
  background: rgba(255, 255, 255, 0.12);
  color: #fff;
  border-bottom-color: var(--color-gold);
}
.header-menu.el-menu--horizontal .el-menu-item.is-active {
  background: rgba(255, 255, 255, 0.12);
  color: #fff;
  border-bottom-color: var(--color-gold);
  font-weight: 600;
}

/* 表格 */
.el-table th.el-table__cell {
  background: var(--color-primary-bg);
  color: var(--color-primary-dark);
  font-weight: 600;
}
.el-table--striped .el-table__body tr.el-table__row--striped td.el-table__cell {
  background: var(--bg-card);
}
.el-table__body tr:hover > td.el-table__cell {
  background: var(--color-earth-bg);
}

/* 卡片 */
.el-card {
  border-radius: var(--radius-md);
  border: 1px solid var(--color-primary-bg);
  transition: all var(--transition-normal);
}
.el-card:hover {
  box-shadow: var(--shadow-card-hover);
}

/* 按钮 */
.el-button--primary {
  --el-button-bg-color: var(--color-primary);
  --el-button-border-color: var(--color-primary);
  --el-button-hover-bg-color: var(--color-primary-dark);
  --el-button-hover-border-color: var(--color-primary-dark);
}
.el-button--success {
  --el-button-bg-color: var(--status-approved);
  --el-button-border-color: var(--status-approved);
}
.el-button--warning {
  --el-button-bg-color: var(--status-pending);
  --el-button-border-color: var(--status-pending);
}
.el-button--danger {
  --el-button-bg-color: var(--status-rejected);
  --el-button-border-color: var(--status-rejected);
}

/* 对话框 */
.el-dialog {
  border-radius: var(--radius-lg);
}
.el-dialog__header {
  background: var(--color-primary-bg);
  border-radius: var(--radius-lg) var(--radius-lg) 0 0;
  padding: 18px 24px;
}
.el-dialog__title {
  color: var(--color-primary-dark);
  font-weight: 600;
  font-size: 16px;
}

/* 输入框焦点 */
.el-input__wrapper:focus-within,
.el-select .el-input__wrapper:focus-within,
.el-textarea__inner:focus-within {
  box-shadow: 0 0 0 2px var(--color-primary-bg);
}

/* 标签 */
.el-tag--success {
  background: var(--color-primary-bg);
  border-color: var(--color-primary);
  color: var(--color-primary-dark);
}
.el-tag--warning {
  background: var(--color-gold-light);
  border-color: var(--color-gold);
  color: var(--color-gold-dark);
}

/* 分页 */
.el-pagination .el-pager li.is-active {
  background-color: var(--color-primary);
}

/* 标签页 */
.el-tabs__item.is-active {
  color: var(--color-primary);
}
.el-tabs__active-bar {
  background-color: var(--color-primary);
}

/* 日期选择器 */
.el-date-table td.current:not(.disabled) .el-date-table-cell__text {
  background-color: var(--color-primary);
}
</style>