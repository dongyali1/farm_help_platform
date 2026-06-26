<template>
  <div class="product-container">
    <div class="search-bar">
      <el-input v-model="searchKeyword" placeholder="搜索商品" @keyup.enter="searchProducts">
        <template #append>
          <el-button @click="searchProducts">搜索</el-button>
        </template>
      </el-input>
      <el-select v-model="selectedCategory" placeholder="选择分类">
        <el-option label="全部" value="" />
        <el-option label="蔬菜水果" value="蔬菜水果" />
        <el-option label="粮油米面" value="粮油米面" />
        <el-option label="肉类禽蛋" value="肉类禽蛋" />
        <el-option label="特色农产品" value="特色农产品" />
      </el-select>
    </div>

    <el-row :gutter="20">
      <el-col :span="6" v-for="product in products" :key="product.id">
        <el-card :body-style="{ padding: '12px' }" class="product-card">
          <img :src="getImageUrl(Array.isArray(product.images) ? product.images[0] : product.images)" class="product-img" />
          <h4>{{ product.name }}</h4>
          <p class="category">{{ product.category }}</p>
          <p class="origin">📍 {{ product.origin || '产地未填' }}</p>
          <p class="price">¥{{ product.price }} 元/kg</p>
          <p class="stock">库存: {{ product.stock }}kg</p>
          <p class="desc">{{ product.description }}</p>

          <!-- 农户联系信息 -->
          <div class="farmer-contact-box">
            <p><strong>👨‍🌾 {{ product.farmer_name }}</strong></p>
            <p class="contact-phone">📱 {{ product.farmer_phone }}</p>
            <p class="contact-addr">🏠 {{ product.farmer_address || '地址未填' }}</p>
          </div>

          <!-- 用户评价区域 -->
          <div class="reviews-section">
            <div class="reviews-header">
              <span class="reviews-title">⭐ 用户评价</span>
              <span class="reviews-count">({{ getProductReviews(product.id).length }}条)</span>
            </div>
            <div class="reviews-content" v-if="getProductReviews(product.id).length > 0">
              <div class="review-item" v-for="review in getProductReviews(product.id).slice(0, 2)" :key="review.id">
                <div class="review-user">{{ review.username }}</div>
                <div class="review-rating">
                  <span v-for="i in 5" :key="i" class="star" :class="{ active: i <= review.rating }">★</span>
                </div>
                <div class="review-content">{{ review.content }}</div>
              </div>
              <div v-if="getProductReviews(product.id).length > 2" class="more-reviews" @click="showAllReviews(product)">查看全部评价</div>
            </div>
            <div class="no-reviews" v-else>暂无评价</div>
          </div>

          <div class="actions">
            <el-button type="primary" size="small" @click="viewDetail(product)">查看详情</el-button>
            <el-button size="small" type="success" @click="openReviewModal(product)">评价</el-button>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 商品详情弹窗 -->
    <el-dialog title="商品详情" v-model="showDetailModal" width="600px">
      <el-row>
        <el-col :span="12">
          <img :src="getImageUrl(selectedProduct && (Array.isArray(selectedProduct.images) ? selectedProduct.images[0] : selectedProduct.images))" class="detail-img" />
        </el-col>
        <el-col :span="12">
          <h3>{{ selectedProduct?.name }}</h3>
          <p class="detail-category">分类: {{ selectedProduct?.category }}</p>
          <p class="detail-origin">📍 产地: {{ selectedProduct?.origin || '未填' }}</p>
          <p class="detail-location">🏠 农产地点: {{ selectedProduct?.location || '未填' }}</p>
          <p class="detail-price">¥{{ selectedProduct?.price }} 元/kg</p>
          <p class="detail-stock">库存: {{ selectedProduct?.stock }}kg</p>
          <p class="detail-desc">描述: {{ selectedProduct?.description }}</p>
          <el-divider />
          <div class="farmer-info-box">
            <h4>👨‍🌾 农户信息</h4>
            <p>姓名: {{ selectedProduct?.farmer_name }}</p>
            <p>📱 电话: {{ selectedProduct?.farmer_phone }}</p>
            <p>🏠 地址: {{ selectedProduct?.farmer_address || '未填' }}</p>
          </div>
        </el-col>
      </el-row>
      <!-- 详情页评价 -->
      <div class="detail-reviews">
        <h4>用户评价 ({{ getProductReviews(selectedProduct?.id).length }}条)</h4>
        <div v-for="review in getProductReviews(selectedProduct?.id)" :key="review.id" class="detail-review-item">
          <div class="review-header">
            <span class="review-username">{{ review.username }}</span>
            <span class="review-rating-stars">
              <span v-for="i in 5" :key="i" class="star" :class="{ active: i <= review.rating }">★</span>
            </span>
          </div>
          <div class="review-text">{{ review.content }}</div>
        </div>
      </div>
    </el-dialog>

    <!-- 评价弹窗 -->
    <el-dialog title="发表评价" v-model="showReviewModal" width="400px">
      <el-form :model="reviewForm" label-width="80px">
        <el-form-item label="商品名称">
          {{ reviewForm.product?.name }}
        </el-form-item>
        <el-form-item label="评分">
          <div class="rating-input">
            <span v-for="i in 5" :key="i" class="star-large" :class="{ active: i <= reviewForm.rating }" @click="reviewForm.rating = i">★</span>
          </div>
        </el-form-item>
        <el-form-item label="评价内容">
          <el-input type="textarea" v-model="reviewForm.content" placeholder="请输入评价内容" :rows="4" resize="vertical" style="width: 100%;" />
        </el-form-item>
      </el-form>
      <div slot="footer">
        <el-button @click="showReviewModal = false">取消</el-button>
        <el-button type="primary" @click="submitReview">提交评价</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import { ElMessage } from 'element-plus'
import { getImageUrl } from '../utils/imageHelper'

const props = defineProps({
  currentUser: { type: Object, default: null }
})

const emit = defineEmits(['showLogin'])
const products = ref([])
const reviews = ref([])
const searchKeyword = ref('')
const selectedCategory = ref('')
const showDetailModal = ref(false)
const showReviewModal = ref(false)
const selectedProduct = ref(null)
const reviewForm = ref({ product: null, rating: 5, content: '' })

const searchProducts = async () => {
  try {
    let url = '/products'
    if (selectedCategory.value) {
      url += `?category=${selectedCategory.value}`
    }
    const [productsRes, reviewsRes] = await Promise.all([
      axios.get(url),
      axios.get('/reviews')
    ])
    products.value = productsRes.data
    reviews.value = reviewsRes.data
    if (searchKeyword.value) {
      products.value = products.value.filter(p => p.name.includes(searchKeyword.value))
    }
  } catch (error) {
    ElMessage.error('获取商品失败')
  }
}

const getProductReviews = (productId) => {
  if (!productId) return []
  return reviews.value.filter(r => r.product_id === productId)
}

const viewDetail = (product) => {
  selectedProduct.value = product
  showDetailModal.value = true
}

const openReviewModal = (product) => {
  if (!props.currentUser) {
    ElMessage.warning('请先登录')
    emit('showLogin')
    return
  }
  reviewForm.value = { product, rating: 5, content: '' }
  showReviewModal.value = true
}

const submitReview = async () => {
  try {
    await axios.post('/reviews', {
      user_id: props.currentUser.id,
      product_id: reviewForm.value.product.id,
      rating: reviewForm.value.rating,
      content: reviewForm.value.content
    })
    ElMessage.success('评价提交成功')
    showReviewModal.value = false
    searchProducts()
  } catch (error) {
    ElMessage.error('提交评价失败')
  }
}

const showAllReviews = (product) => {
  selectedProduct.value = product
  showDetailModal.value = true
}

onMounted(() => { searchProducts() })
</script>

<style scoped>
.product-container { padding: 24px; }
.search-bar { display: flex; gap: 20px; margin-bottom: 24px; }

.product-card { height: 100%; transition: transform var(--transition-normal); }
.product-card:hover { transform: translateY(-4px); }
.product-img { width: 100%; height: 150px; object-fit: cover; border-radius: var(--radius-sm); }

.category { color: var(--color-primary); font-size: 12px; font-weight: 600; }
.origin { color: var(--color-earth); font-size: 12px; margin-top: 2px; }
.price { color: var(--text-price); font-weight: bold; font-size: 20px; }
.stock { color: var(--text-muted); font-size: 12px; }
.desc { color: var(--text-secondary); font-size: 12px; overflow: hidden; text-overflow: ellipsis; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; }

.farmer-contact-box { margin-top: 10px; padding: 10px; background: var(--bg-grey); border-radius: var(--radius-sm); font-size: 12px; }
.farmer-contact-box p { margin: 3px 0; }
.contact-phone { color: var(--color-primary); }
.contact-addr { color: var(--text-muted); font-size: 11px; }

.reviews-section { margin-top: 10px; padding-top: 10px; border-top: 1px solid var(--color-primary-bg); }
.reviews-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 8px; }
.reviews-title { font-size: 13px; font-weight: bold; color: var(--text-primary); }
.reviews-count { font-size: 12px; color: var(--text-muted); }
.reviews-content { max-height: 120px; overflow-y: auto; }
.review-item { padding: 6px 0; border-bottom: 1px dashed var(--color-primary-bg); }
.review-item:last-child { border-bottom: none; }
.review-user { font-size: 12px; color: var(--color-primary); margin-bottom: 4px; }
.review-rating { margin-bottom: 4px; }
.star { font-size: 12px; color: var(--star-inactive); }
.star.active { color: var(--star-color); }
.review-content { font-size: 12px; color: var(--text-secondary); line-height: 1.4; }
.more-reviews { font-size: 12px; color: var(--color-link); cursor: pointer; text-align: center; padding: 4px 0; }
.no-reviews { font-size: 12px; color: var(--text-muted); text-align: center; padding: 10px 0; }
.actions { display: flex; gap: 8px; margin-top: 10px; }

.detail-img { width: 100%; height: 250px; object-fit: cover; border-radius: var(--radius-sm); }
.detail-category { color: var(--color-primary); }
.detail-origin { color: var(--color-earth); font-size: 13px; }
.detail-location { color: var(--text-secondary); font-size: 13px; }
.detail-price { color: var(--text-price); font-size: 24px; font-weight: bold; }
.detail-stock { color: var(--text-muted); }
.detail-desc { color: var(--text-secondary); line-height: 1.6; }
.farmer-info-box { padding: 12px; background: var(--bg-grey); border-radius: var(--radius-sm); }
.farmer-info-box h4 { color: var(--color-primary); margin-bottom: 8px; }
.farmer-info-box p { margin: 4px 0; color: var(--text-primary); font-size: 14px; }
.detail-reviews { margin-top: 24px; padding-top: 24px; border-top: 1px solid var(--color-primary-bg); }
.detail-review-item { padding: 12px 0; border-bottom: 1px dashed var(--color-primary-bg); }
.review-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 8px; }
.review-username { font-size: 14px; color: var(--color-primary); font-weight: 500; }
.review-text { font-size: 14px; color: var(--text-secondary); line-height: 1.5; }
.review-rating-stars { font-size: 14px; }

.rating-input { display: flex; gap: 5px; }
.star-large { font-size: 26px; color: var(--star-inactive); cursor: pointer; transition: color var(--transition-fast), transform var(--transition-fast); }
.star-large:hover { transform: scale(1.15); }
.star-large.active { color: var(--star-color); }
</style>
