<template>
  <div class="home-container">
    <div class="hero-section">
      <h1>🌾 助农平台 — 供需对接，直连产地</h1>
      <p>消费者找好货，农户找销路，让优质农产品没有中间商</p>
    </div>

    <div class="stats-section">
      <el-row :gutter="20">
        <el-col :span="6">
          <div class="stat-card" @click="navigateTo('商品列表')">
            <div class="stat-icon">📦</div>
            <div class="stat-number">{{ stats.products }}</div>
            <div class="stat-label">在售商品</div>
            <div class="stat-arrow">→</div>
          </div>
        </el-col>
        <el-col :span="6">
          <div class="stat-card" @click="navigateTo('入驻农户')">
            <div class="stat-icon">👨‍🌾</div>
            <div class="stat-number">{{ stats.farmers }}</div>
            <div class="stat-label">入驻农户</div>
            <div class="stat-arrow">→</div>
          </div>
        </el-col>
        <el-col :span="6">
          <div class="stat-card" @click="navigateTo('需求广场')">
            <div class="stat-icon">📋</div>
            <div class="stat-number">{{ stats.demands }}</div>
            <div class="stat-label">采购需求</div>
            <div class="stat-arrow">→</div>
          </div>
        </el-col>
        <el-col :span="6">
          <div class="stat-card" @click="navigateTo('用户评价')">
            <div class="stat-icon">⭐</div>
            <div class="stat-number">{{ stats.reviews }}</div>
            <div class="stat-label">用户评价</div>
            <div class="stat-arrow">→</div>
          </div>
        </el-col>
      </el-row>
    </div>

    <div class="products-section">
      <h2>热门商品</h2>
      <el-row :gutter="20">
        <el-col :span="6" v-for="product in hotProducts" :key="product.id">
          <el-card :body-style="{ padding: '12px' }">
            <img :src="getImageUrl(product.images[0])" class="product-img" />
            <h4>{{ product.name }}</h4>
            <p class="origin">📍 {{ product.origin || '产地未填' }}</p>
            <p class="price">¥{{ product.price }} 元/kg</p>
            <p class="desc">{{ product.description }}</p>
            <p class="farmer-contact">👨‍🌾 {{ product.farmer_name }} · 📱 {{ product.farmer_phone }}</p>
          </el-card>
        </el-col>
      </el-row>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import { ElMessage } from 'element-plus'
import { getImageUrl } from '../utils/imageHelper'

const emit = defineEmits(['navigate'])

const stats = ref({
  products: 0,
  farmers: 0,
  demands: 0,
  reviews: 0
})

const hotProducts = ref([])

const navigateTo = (section) => {
  emit('navigate', section)
  ElMessage.info(`正在跳转到${section}页面...`)
}

onMounted(async () => {
  try {
    const [productsRes, statsRes] = await Promise.all([
      axios.get('/products'),
      axios.get('/stats/dashboard')
    ])
    hotProducts.value = productsRes.data.slice(0, 4)
    stats.value = {
      products: statsRes.data.total_products,
      farmers: statsRes.data.total_farmers,
      demands: statsRes.data.total_demands,
      reviews: statsRes.data.total_reviews
    }
  } catch (error) {
    console.error('获取数据失败:', error)
  }
})
</script>

<style scoped>
.home-container {
  padding: 24px;
}

/* === 英雄区 === */
.hero-section {
  background:
    radial-gradient(circle at 20% 30%, rgba(255, 255, 255, 0.06) 0%, transparent 50%),
    radial-gradient(circle at 80% 70%, rgba(240, 165, 0, 0.08) 0%, transparent 40%),
    linear-gradient(135deg, var(--color-primary) 0%, var(--color-primary-dark) 100%);
  padding: 70px 60px;
  text-align: center;
  color: white;
  border-radius: var(--radius-lg);
  margin-bottom: 32px;
  box-shadow: var(--shadow-green);
  position: relative;
  overflow: hidden;
}
.hero-section::before {
  content: '';
  position: absolute;
  bottom: -30px;
  left: 0;
  right: 0;
  height: 60px;
  background: linear-gradient(transparent, rgba(255, 255, 255, 0.04));
  border-radius: 0 0 var(--radius-lg) var(--radius-lg);
}
.hero-section h1 {
  font-size: 38px;
  margin-bottom: 12px;
  text-shadow: 0 2px 6px rgba(0, 0, 0, 0.2);
}
.hero-section p {
  font-size: 18px;
  opacity: 0.9;
}

/* === 统计区 === */
.stats-section {
  margin-bottom: 32px;
}
.stat-card {
  background: var(--bg-card);
  padding: 24px 20px;
  text-align: center;
  border-radius: var(--radius-md);
  cursor: pointer;
  transition: transform var(--transition-normal), box-shadow var(--transition-normal);
  position: relative;
  border: 1px solid var(--color-primary-bg);
  box-shadow: var(--shadow-card);
}
.stat-card:hover {
  transform: translateY(-6px);
  box-shadow: var(--shadow-card-hover);
  border-color: var(--color-primary-light);
}
.stat-icon {
  font-size: 40px;
  margin-bottom: 12px;
}
.stat-number {
  font-size: 34px;
  font-weight: bold;
  color: var(--color-primary);
  margin-bottom: 4px;
}
.stat-label {
  color: var(--text-muted);
  font-size: 14px;
}
.stat-arrow {
  position: absolute;
  top: 12px;
  right: 14px;
  font-size: 20px;
  color: var(--text-muted);
  opacity: 0.4;
  transition: opacity var(--transition-fast);
}
.stat-card:hover .stat-arrow {
  opacity: 1;
  color: var(--color-primary);
}

/* === 热门商品区 === */
.products-section {
  margin-bottom: 32px;
}
.products-section h2 {
  color: var(--text-primary);
  font-size: 22px;
  margin-bottom: 20px;
  padding-bottom: 10px;
  border-bottom: 3px solid var(--color-primary-bg);
  display: inline-block;
}
.product-img {
  width: 100%;
  height: 160px;
  object-fit: cover;
  border-radius: var(--radius-sm);
}
.origin {
  color: var(--color-earth);
  font-size: 12px;
  margin-top: 4px;
}
.price {
  color: var(--text-price);
  font-weight: bold;
  font-size: 20px;
}
.desc {
  color: var(--text-muted);
  font-size: 12px;
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
}
.farmer-contact {
  color: var(--text-secondary);
  font-size: 12px;
  margin-top: 8px;
  padding-top: 8px;
  border-top: 1px dashed var(--color-primary-bg);
}
</style>
