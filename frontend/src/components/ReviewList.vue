<template>
  <div class="review-container">
    <div class="filter-bar">
      <el-select v-model="ratingFilter" placeholder="筛选评分">
        <el-option label="全部" value="" />
        <el-option label="5星好评" value="5" />
        <el-option label="4星" value="4" />
        <el-option label="3星" value="3" />
        <el-option label="2星" value="2" />
        <el-option label="1星差评" value="1" />
      </el-select>
    </div>

    <el-row :gutter="20">
      <el-col :span="12" v-for="review in filteredReviews" :key="review.id">
        <el-card :body-style="{ padding: '20px' }" class="review-card">
          <div class="review-header">
            <div class="reviewer-info">
              <span class="reviewer-avatar">👤</span>
              <span class="reviewer-name">{{ review.username }}</span>
            </div>
            <div class="review-rating">
              <span v-for="i in 5" :key="i" class="star">
                {{ i <= review.rating ? '⭐' : '☆' }}
              </span>
            </div>
          </div>
          <p class="review-product">{{ review.product_name }}</p>
          <p class="review-content">{{ review.content }}</p>
          <p class="review-time">{{ formatDate(review.created_at) }}</p>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import axios from 'axios'
import { ElMessage } from 'element-plus'

const reviews = ref([])
const ratingFilter = ref('')

const filteredReviews = computed(() => {
  if (!ratingFilter.value) return reviews.value
  return reviews.value.filter(r => r.rating === parseInt(ratingFilter.value))
})

const getReviews = async () => {
  try {
    const response = await axios.get('/reviews')
    reviews.value = response.data
  } catch (error) {
    ElMessage.error('获取评价失败')
  }
}

const formatDate = (dateStr) => {
  if (!dateStr) return ''
  const date = new Date(dateStr)
  return `${date.getFullYear()}-${String(date.getMonth()+1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`
}

onMounted(() => {
  getReviews()
})
</script>

<style scoped>
.review-container {
  padding: 24px;
}
.filter-bar {
  margin-bottom: 24px;
}
.review-card {
  margin-bottom: 20px;
  transition: box-shadow var(--transition-normal);
}
.review-card:hover {
  box-shadow: var(--shadow-card-hover);
}
.review-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}
.reviewer-info {
  display: flex;
  align-items: center;
  gap: 10px;
}
.reviewer-avatar {
  font-size: 28px;
}
.reviewer-name {
  font-weight: bold;
  color: var(--text-primary);
}
.review-rating {
  font-size: 18px;
}
.review-product {
  color: var(--color-primary);
  font-size: 14px;
  font-weight: 500;
}
.review-content {
  color: var(--text-secondary);
  line-height: 1.6;
}
.review-time {
  color: var(--text-muted);
  font-size: 12px;
  margin-top: 12px;
}
</style>