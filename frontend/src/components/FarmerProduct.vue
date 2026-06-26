<template>
  <div class="farmer-product-container">
    <h2>🌾 农户商品管理</h2>
    
    <!-- 商品列表 -->
    <div class="product-section">
      <h3>我的商品</h3>
      <el-row :gutter="20">
        <el-col :span="6" v-for="product in myProducts" :key="product.id">
          <el-card :body-style="{ padding: '10px' }" class="product-card">
            <div class="status-tag">
              <el-tag :type="getStatusType(product.status)" size="small">
                {{ getStatusText(product.status) }}
              </el-tag>
            </div>
            <img :src="getImageUrl(Array.isArray(product.images) ? product.images[0] : product.images)" class="product-img" />
            <h4>{{ product.name }}</h4>
            <p class="price">¥{{ product.price }} 元/kg</p>
            <p class="stock">库存: {{ product.stock }}kg</p>
            <div class="description-preview" v-if="product.description" @click="viewFullDescription(product)">
              <p class="description-text">{{ truncateDescription(product.description, 30) }}</p>
              <span class="view-more" v-if="product.description.length > 30">点击查看详情</span>
            </div>
            <p class="create-time">发布时间: {{ product.created_at }}</p>
            <div class="actions">
              <el-button size="small" @click="editProduct(product)">编辑</el-button>
              <el-button size="small" type="danger" @click="deleteProduct(product)">删除</el-button>
            </div>
          </el-card>
        </el-col>
      </el-row>
    </div>
    
    <!-- 上传商品表单 -->
    <div class="upload-section">
      <h3>📤 上传新商品</h3>
      <el-form :model="productForm" label-width="100px" class="product-form">
        <el-form-item label="商品名称">
          <el-input v-model="productForm.name" placeholder="请输入商品名称" />
        </el-form-item>
        <el-form-item label="商品分类">
          <el-select v-model="productForm.category">
            <el-option label="蔬菜水果" value="蔬菜水果" />
            <el-option label="粮油米面" value="粮油米面" />
            <el-option label="肉类禽蛋" value="肉类禽蛋" />
            <el-option label="特色农产品" value="特色农产品" />
          </el-select>
        </el-form-item>
        <el-form-item label="商品价格(元/kg)">
          <el-input v-model.number="productForm.price" placeholder="请输入价格" type="number" />
        </el-form-item>
        <el-form-item label="库存数量(kg)">
          <el-input v-model.number="productForm.stock" placeholder="请输入库存" type="number" />
        </el-form-item>
        <el-form-item label="产地">
          <el-input v-model="productForm.origin" placeholder="例如：山东济南" />
        </el-form-item>
        <el-form-item label="农产地点">
          <el-input v-model="productForm.location" placeholder="例如：济南市历城区王舍人镇" />
        </el-form-item>
        <el-form-item label="商品描述">
          <el-input
            type="textarea"
            v-model="productForm.description"
            placeholder="请详细描述商品的产地、特点、种植/养殖方式等信息，字数不超过200字"
            :rows="3"
            maxlength="200"
            show-word-limit
            @input="handleDescriptionInput('product')"
          />
          <div class="description-tips">
            <span :class="{'warning': productForm.description.length > 180}">
              已输入 {{ productForm.description.length }} / 200 字
            </span>
          </div>
        </el-form-item>
        <el-form-item label="商品图片">
          <div class="upload-area">
            <el-upload
              class="upload-demo"
              action="http://localhost:5000/api/upload"
              :on-success="handleUploadSuccess"
              :on-error="handleUploadError"
              :before-upload="beforeUpload"
              :file-list="imageFileList"
              list-type="picture-card"
              :limit="3"
            >
              <i class="el-icon-plus"></i>
            </el-upload>
            <p v-if="productForm.images" class="upload-tip">已上传 {{ productForm.images.split(',').length }} 张图片</p>
          </div>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="submitProduct">上传商品</el-button>
          <el-button @click="resetForm">重置</el-button>
        </el-form-item>
      </el-form>
    </div>
    
    <!-- 查看完整描述弹窗 -->
    <el-dialog title="商品描述" v-model="showDescriptionModal" width="600px">
      <div class="full-description">
        <p>{{ fullDescription }}</p>
      </div>
      <div slot="footer">
        <el-button @click="showDescriptionModal = false">关闭</el-button>
      </div>
    </el-dialog>

    <!-- 编辑商品弹窗 -->
    <el-dialog title="编辑商品" v-model="showEditModal" width="500px">
      <el-form :model="editForm" label-width="100px">
        <el-form-item label="商品名称">
          <el-input v-model="editForm.name" />
        </el-form-item>
        <el-form-item label="商品分类">
          <el-select v-model="editForm.category">
            <el-option label="蔬菜水果" value="蔬菜水果" />
            <el-option label="粮油米面" value="粮油米面" />
            <el-option label="肉类禽蛋" value="肉类禽蛋" />
            <el-option label="特色农产品" value="特色农产品" />
          </el-select>
        </el-form-item>
        <el-form-item label="商品价格(元/kg)">
          <el-input v-model.number="editForm.price" type="number" />
        </el-form-item>
        <el-form-item label="库存数量(kg)">
          <el-input v-model.number="editForm.stock" type="number" />
        </el-form-item>
        <el-form-item label="产地">
          <el-input v-model="editForm.origin" placeholder="例如：山东济南" />
        </el-form-item>
        <el-form-item label="农产地点">
          <el-input v-model="editForm.location" placeholder="例如：济南市历城区王舍人镇" />
        </el-form-item>
        <el-form-item label="商品描述">
          <el-input
            type="textarea"
            v-model="editForm.description"
            placeholder="请详细描述商品的产地、特点、种植/养殖方式等信息，字数不超过200字"
            :rows="3"
            maxlength="200"
            show-word-limit
          />
          <div class="description-tips">
            <span :class="{'warning': editForm.description.length > 180}">
              已输入 {{ editForm.description.length }} / 200 字
            </span>
          </div>
        </el-form-item>
        <el-form-item label="商品图片">
          <div class="upload-area">
            <el-upload
              class="upload-demo"
              action="http://localhost:5000/api/upload"
              :on-success="handleEditUploadSuccess"
              :on-error="handleUploadError"
              :before-upload="beforeUpload"
              :file-list="editImageFileList"
              list-type="picture-card"
              :limit="3"
            >
              <i class="el-icon-plus"></i>
            </el-upload>
          </div>
        </el-form-item>
      </el-form>
      <div slot="footer">
        <el-button @click="showEditModal = false">取消</el-button>
        <el-button type="primary" @click="updateProduct">保存修改</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import axios from 'axios'
import { ElMessage } from 'element-plus'

const props = defineProps({
  currentUser: {
    type: Object,
    default: null
  }
})

const myProducts = ref([])
const showEditModal = ref(false)
const showDescriptionModal = ref(false)
const fullDescription = ref('')
const editingProduct = ref(null)
const imageFileList = ref([])
const editImageFileList = ref([])

const productForm = ref({
  name: '',
  category: '蔬菜水果',
  price: 0,
  stock: 0,
  description: '',
  origin: '',
  location: '',
  images: ''
})

const editForm = ref({
  name: '',
  category: '',
  price: 0,
  stock: 0,
  description: '',
  origin: '',
  location: '',
  images: ''
})

const getImageUrl = (url) => {
  if (!url) return 'https://via.placeholder.com/200'
  if (url.startsWith('/uploads/')) {
    return `http://localhost:5000${url}`
  }
  return url
}

const getStatusType = (status) => {
  switch (status) {
    case 'pending':
      return 'warning'
    case 'approved':
      return 'success'
    case 'rejected':
      return 'danger'
    case 'offline':
      return 'info'
    default:
      return 'default'
  }
}

const getStatusText = (status) => {
  switch (status) {
    case 'pending':
      return '待审核'
    case 'approved':
      return '已通过'
    case 'rejected':
      return '已拒绝'
    case 'offline':
      return '已下架'
    default:
      return status
  }
}

const truncateDescription = (text, maxLength) => {
  if (!text) return ''
  if (text.length <= maxLength) return text
  return text.substring(0, maxLength) + '...'
}

const viewFullDescription = (product) => {
  fullDescription.value = product.description
  showDescriptionModal.value = true
}

const loadMyProducts = async () => {
  if (!props.currentUser) return
  
  try {
    const response = await axios.get(`/farmer/products?farmer_id=${props.currentUser.id}`)
    myProducts.value = response.data
  } catch (error) {
    ElMessage.error('获取商品失败')
  }
}

const beforeUpload = (file) => {
  const isImage = file.type.startsWith('image/')
  if (!isImage) {
    ElMessage.error('只能上传图片文件')
    return false
  }
  const isLt2M = file.size / 1024 / 1024 < 2
  if (!isLt2M) {
    ElMessage.error('图片大小不能超过2MB')
    return false
  }
  return true
}

const handleUploadSuccess = (response) => {
  if (response.url) {
    if (productForm.value.images) {
      productForm.value.images += ',' + response.url
    } else {
      productForm.value.images = response.url
    }
    ElMessage.success('图片上传成功')
  }
}

const handleEditUploadSuccess = (response) => {
  if (response.url) {
    if (editForm.value.images) {
      editForm.value.images += ',' + response.url
    } else {
      editForm.value.images = response.url
    }
    ElMessage.success('图片上传成功')
  }
}

const handleUploadError = () => {
  ElMessage.error('图片上传失败')
}

const handleDescriptionInput = (form) => {
  // 描述输入处理，可扩展自定义逻辑
}

const submitProduct = async () => {
  if (!props.currentUser) {
    ElMessage.warning('请先登录')
    return
  }
  
  if (!productForm.value.images) {
    ElMessage.warning('请至少上传一张商品图片')
    return
  }
  
  try {
    await axios.post('/products', {
      ...productForm.value,
      farmer_id: props.currentUser.id,
      status: 'pending'
    })
    ElMessage.success('商品上传成功，等待审核')
    resetForm()
    loadMyProducts()
  } catch (error) {
    ElMessage.error(error.response?.data?.error || '上传失败')
  }
}

const resetForm = () => {
  productForm.value = {
    name: '',
    category: '蔬菜水果',
    price: 0,
    stock: 0,
    description: '',
    origin: '',
    location: '',
    images: ''
  }
  imageFileList.value = []
}

const editProduct = (product) => {
  editingProduct.value = product
  editForm.value = {
    name: product.name,
    category: product.category,
    price: product.price,
    stock: product.stock,
    description: product.description,
    origin: product.origin || '',
    location: product.location || '',
    images: Array.isArray(product.images) ? product.images.join(',') : (product.images || '')
  }
  // 设置已有的图片文件列表
  if (editForm.value.images) {
    const urls = editForm.value.images.split(',')
    editImageFileList.value = urls.map(url => ({
      name: url.split('/').pop(),
      url: getImageUrl(url),
      status: 'success'
    }))
  } else {
    editImageFileList.value = []
  }
  showEditModal.value = true
}

const updateProduct = async () => {
  try {
    await axios.put(`/products/${editingProduct.value.id}`, editForm.value)
    ElMessage.success('商品更新成功')
    showEditModal.value = false
    loadMyProducts()
  } catch (error) {
    ElMessage.error('更新失败')
  }
}

const deleteProduct = async (product) => {
  try {
    await axios.delete(`/products/${product.id}`)
    ElMessage.success('商品删除成功')
    loadMyProducts()
  } catch (error) {
    ElMessage.error('删除失败')
  }
}

watch(() => props.currentUser, () => {
  loadMyProducts()
}, { deep: true })

onMounted(() => {
  loadMyProducts()
})
</script>

<style scoped>
.farmer-product-container {
  padding: 24px;
}
.farmer-product-container h2 {
  color: var(--color-primary);
  margin-bottom: 20px;
}

.product-section {
  margin-bottom: 36px;
}
.product-section h3 {
  color: var(--text-primary);
  margin-bottom: 16px;
  font-size: 18px;
}

/* === 商品卡片 === */
.product-card {
  height: 100%;
  position: relative;
  transition: transform var(--transition-normal);
}
.product-card:hover {
  transform: translateY(-3px);
}
.status-tag {
  position: absolute;
  top: 10px;
  right: 10px;
  z-index: 10;
}
.product-img {
  width: 100%;
  height: 150px;
  object-fit: cover;
  border-radius: var(--radius-sm);
}

.price {
  color: var(--text-price);
  font-weight: bold;
  font-size: 18px;
}
.stock {
  color: var(--text-muted);
  font-size: 12px;
}
.create-time {
  color: var(--text-muted);
  font-size: 11px;
  margin-top: 5px;
}
.actions {
  display: flex;
  gap: 10px;
  margin-top: 12px;
}

/* === 上传区域 === */
.upload-section {
  background: var(--bg-grey);
  padding: 24px;
  border-radius: var(--radius-md);
  border: 1px solid var(--color-primary-bg);
}
.upload-section h3 {
  color: var(--text-primary);
  margin-bottom: 16px;
}
.product-form {
  max-width: 600px;
}
.upload-area {
  width: 100%;
}
.upload-tip {
  color: var(--text-muted);
  font-size: 12px;
  margin-top: 10px;
}

/* === 描述预览 === */
.description-preview {
  margin: 8px 0;
  padding: 8px;
  background: var(--bg-grey);
  border-radius: var(--radius-sm);
}
.description-text {
  font-size: 12px;
  color: var(--text-secondary);
  line-height: 1.4;
  margin: 0;
  word-break: break-word;
}
.description-tips {
  font-size: 12px;
  color: var(--text-muted);
  margin-top: 5px;
}
.description-tips .warning {
  color: var(--status-pending);
  font-weight: 500;
}
.view-more {
  font-size: 11px;
  color: var(--color-link);
  cursor: pointer;
  display: block;
  margin-top: 4px;
}
.view-more:hover {
  text-decoration: underline;
}
.full-description {
  padding: 20px;
  background: var(--bg-grey);
  border-radius: var(--radius-md);
  line-height: 1.8;
  font-size: 14px;
  color: var(--text-primary);
  word-break: break-word;
  max-height: 400px;
  overflow-y: auto;
}
.full-description p {
  margin: 0;
  white-space: pre-wrap;
}
</style>