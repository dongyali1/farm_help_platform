<template>
  <div class="farmer-container">
    <div class="search-bar">
      <el-input v-model="searchKeyword" placeholder="搜索农户" @keyup.enter="searchFarmers">
        <template #append>
          <el-button @click="searchFarmers">搜索</el-button>
        </template>
      </el-input>
    </div>

    <el-row :gutter="20">
      <el-col :span="8" v-for="farmer in farmers" :key="farmer.id">
        <el-card :body-style="{ padding: '20px' }" class="farmer-card">
          <div class="farmer-avatar">👨‍🌾</div>
          <h3>{{ farmer.username }}</h3>
          <p class="farmer-phone">📱 {{ farmer.phone }}</p>
          <p class="farmer-role">角色: {{ farmer.role === 'farmer' ? '农户' : '消费者' }}</p>
          <p class="farmer-join">入驻时间: {{ formatDate(farmer.created_at) }}</p>
          <div class="actions">
            <el-button type="primary" size="small" @click="viewProducts(farmer)">查看商品</el-button>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 农户商品弹窗 -->
    <el-dialog :title="`${selectedFarmer?.username} 的商品`" v-model="showProductModal" width="800px">
      <div v-if="farmerProducts.length > 0">
        <el-row :gutter="15">
          <el-col :span="12" v-for="product in farmerProducts" :key="product.id">
            <el-card :body-style="{ padding: '10px' }">
              <img :src="getImageUrl(Array.isArray(product.images) ? product.images[0] : product.images)" class="product-img" />
              <h4>{{ product.name }}</h4>
              <p class="product-origin">📍 {{ product.origin || '产地未填' }}</p>
              <p class="product-price">¥{{ product.price }} 元/kg</p>
              <p class="product-stock">库存: {{ product.stock }}kg</p>
              <p class="product-desc">{{ product.description }}</p>
              <p class="product-contact">📱 {{ product.farmer_phone }}</p>
            </el-card>
          </el-col>
        </el-row>
      </div>
      <div v-else class="no-products">
        <p style="text-align: center; color: #999;">该农户暂无商品</p>
      </div>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import { getImageUrl } from '../utils/imageHelper'
import { ElMessage } from 'element-plus'

const emit = defineEmits(['navigate'])

const farmers = ref([])
const searchKeyword = ref('')
const showProductModal = ref(false)
const selectedFarmer = ref(null)
const farmerProducts = ref([])

const searchFarmers = async () => {
  try {
    const response = await axios.get('/users')
    let data = response.data
    if (searchKeyword.value) {
      data = data.filter(f => f.username.includes(searchKeyword.value))
    }
    farmers.value = data.filter(f => f.role === 'farmer')
  } catch (error) {
    ElMessage.error('获取农户信息失败')
  }
}

const viewProducts = async (farmer) => {
  selectedFarmer.value = farmer
  showProductModal.value = true
  
  try {
    const response = await axios.get(`/products?farmer_id=${farmer.id}`)
    farmerProducts.value = response.data
  } catch (error) {
    ElMessage.error('获取商品失败')
    farmerProducts.value = []
  }
}

const formatDate = (dateStr) => {
  if (!dateStr) return ''
  const date = new Date(dateStr)
  return `${date.getFullYear()}-${String(date.getMonth()+1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`
}

onMounted(() => {
  searchFarmers()
})
</script>

<style scoped>
.farmer-container {
  padding: 24px;
}
.search-bar {
  margin-bottom: 24px;
}
.farmer-card {
  text-align: center;
  transition: transform var(--transition-normal);
}
.farmer-card:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-card-hover);
}
.farmer-avatar {
  font-size: 64px;
  margin-bottom: 12px;
}
.farmer-phone {
  color: var(--color-primary);
}
.farmer-role {
  color: var(--text-secondary);
  font-size: 14px;
}
.farmer-join {
  color: var(--text-muted);
  font-size: 12px;
}
.actions {
  margin-top: 16px;
}
.product-img {
  width: 100%;
  height: 120px;
  object-fit: cover;
  border-radius: var(--radius-sm);
}
.product-origin {
  color: var(--color-earth);
  font-size: 12px;
  margin-top: 4px;
}
.product-price {
  color: var(--text-price);
  font-weight: bold;
  font-size: 18px;
}
.product-contact {
  color: var(--color-primary);
  font-size: 12px;
  margin-top: 6px;
}
.product-stock {
  color: var(--text-muted);
  font-size: 12px;
}
.product-desc {
  color: var(--text-secondary);
  font-size: 12px;
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
}
.no-products {
  padding: 40px;
}
</style>