<template>
  <div class="admin-container">
    <el-tabs v-model="activeTab">
      <!-- 数据统计 -->
      <el-tab-pane label="数据统计" name="stats">
        <el-row :gutter="20">
          <el-col :span="8">
            <div class="stat-card stat-card-green" @click="goTab('products')">
              <div class="stat-icon">📦</div>
              <div class="stat-num">{{ productCount }}</div>
              <div class="stat-text">商品总数</div>
              <div class="stat-hint">点击管理 →</div>
            </div>
          </el-col>
          <el-col :span="8">
            <div class="stat-card stat-card-gold" @click="goTab('products', 'pending')">
              <div class="stat-icon">📝</div>
              <div class="stat-num">{{ pendingProducts.length }}</div>
              <div class="stat-text">待审核商品</div>
              <div class="stat-hint">点击审核 →</div>
            </div>
          </el-col>
          <el-col :span="8">
            <div class="stat-card stat-card-green-light" @click="goTab('users', 'farmer')">
              <div class="stat-icon">👨‍🌾</div>
              <div class="stat-num">{{ farmerCount }}</div>
              <div class="stat-text">入驻农户</div>
              <div class="stat-hint">点击管理 →</div>
            </div>
          </el-col>
        </el-row>
        <el-row :gutter="20" style="margin-top: 20px;">
          <el-col :span="8">
            <div class="stat-card stat-card-warm" @click="goTab('reviews')">
              <div class="stat-icon">⭐</div>
              <div class="stat-num">{{ reviewCount }}</div>
              <div class="stat-text">用户评价</div>
              <div class="stat-hint">点击管理 →</div>
            </div>
          </el-col>
          <el-col :span="8">
            <div class="stat-card stat-card-sky" @click="goTab('purchases')">
              <div class="stat-icon">📋</div>
              <div class="stat-num">{{ demandCount }}</div>
              <div class="stat-text">需求总数</div>
              <div class="stat-hint">点击查看 →</div>
            </div>
          </el-col>
          <el-col :span="8">
            <div class="stat-card stat-card-soil" @click="goTab('users')">
              <div class="stat-icon">👥</div>
              <div class="stat-num">{{ userCount }}</div>
              <div class="stat-text">注册用户</div>
              <div class="stat-hint">点击管理 →</div>
            </div>
          </el-col>
        </el-row>
        <el-row style="margin-top: 20px;">
          <el-col :span="12">
            <div class="chart-card">
              <h4>商品状态分布</h4>
              <el-row :gutter="10">
                <el-col :span="6" v-for="(count, status) in productStatusStats" :key="status">
                  <div class="status-item">
                    <div class="status-bar" :style="{ height: getStatusBarHeight(count) + '%', background: getProductStatusColor(status) }"></div>
                    <div class="status-label">{{ getProductStatusText(status) }}</div>
                    <div class="status-count">{{ count }}</div>
                  </div>
                </el-col>
              </el-row>
            </div>
          </el-col>
          <el-col :span="12">
            <div class="chart-card">
              <h4>用户角色分布</h4>
              <el-row :gutter="10">
                <el-col :span="6" v-for="(count, role) in userRoleStats" :key="role">
                  <div class="role-item">
                    <div class="role-circle" :style="{ background: getRoleColor(role) }">
                      {{ getRoleIcon(role) }}
                    </div>
                    <div class="role-label">{{ getRoleText(role) }}</div>
                    <div class="role-count">{{ count }}</div>
                  </div>
                </el-col>
              </el-row>
            </div>
          </el-col>
        </el-row>
      </el-tab-pane>
      
      <!-- 商品审核 -->
      <el-tab-pane label="商品审核" name="products">
        <div class="filter-bar">
          <el-select v-model="productStatusFilter" placeholder="筛选状态">
            <el-option label="全部" value="" />
            <el-option label="待审核" value="pending" />
            <el-option label="已通过" value="approved" />
            <el-option label="已拒绝" value="rejected" />
          </el-select>
          <el-input v-model="productSearchKeyword" placeholder="搜索商品名称" @keyup.enter="filterProducts" style="width: 200px; margin-left: 10px;" />
          <el-button @click="filterProducts">搜索</el-button>
        </div>
        <el-table :data="filteredProducts" border>
          <el-table-column prop="id" label="ID" />
          <el-table-column prop="name" label="商品名称" />
          <el-table-column prop="category" label="分类" />
          <el-table-column prop="price" label="价格" />
          <el-table-column prop="stock" label="库存(kg)" />
          <el-table-column prop="farmer_name" label="农户名称" />
          <el-table-column prop="status" label="状态">
            <template #default="scope">
              <el-tag :type="getProductStatusType(scope.row.status)">
                {{ getProductStatusText(scope.row.status) }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="created_at" label="提交时间" />
          <el-table-column label="操作">
            <template #default="scope">
              <el-button size="small" @click="viewProductDetail(scope.row)">详情</el-button>
              <el-button v-if="scope.row.status === 'pending'" size="small" type="success" @click="approveProduct(scope.row.id)">通过</el-button>
              <el-button v-if="scope.row.status === 'pending'" size="small" type="danger" @click="openRejectModal(scope.row)">拒绝</el-button>
              <el-button v-if="scope.row.status === 'approved'" size="small" type="warning" @click="offlineProduct(scope.row.id)">下架</el-button>
            </template>
          </el-table-column>
        </el-table>
        
        <!-- 商品详情弹窗 -->
        <el-dialog title="商品详情" v-model="showProductDetailModal" width="600px">
          <div v-if="selectedProduct" class="product-detail">
            <el-row>
              <el-col :span="12">
                <img :src="getImageUrl(Array.isArray(selectedProduct.images) ? selectedProduct.images[0] : selectedProduct.images)" class="detail-img" />
              </el-col>
              <el-col :span="12">
                <h3>{{ selectedProduct.name }}</h3>
                <p>分类: {{ selectedProduct.category }}</p>
                <p>价格: ¥{{ selectedProduct.price }} 元/kg</p>
                <p>库存: {{ selectedProduct.stock }}kg</p>
                <p>农户: {{ selectedProduct.farmer_name }}</p>
                <p>状态: <el-tag :type="getProductStatusType(selectedProduct.status)">{{ getProductStatusText(selectedProduct.status) }}</el-tag></p>
              </el-col>
            </el-row>
            <el-divider></el-divider>
            <p><strong>商品描述:</strong></p>
            <p>{{ selectedProduct.description }}</p>
          </div>
        </el-dialog>
        
        <!-- 拒绝原因弹窗 -->
        <el-dialog title="拒绝商品" v-model="showRejectModal">
          <el-form :model="rejectForm">
            <el-form-item label="拒绝原因">
              <el-textarea v-model="rejectForm.reason" placeholder="请输入拒绝原因" :rows="3" />
            </el-form-item>
          </el-form>
          <div slot="footer">
            <el-button @click="showRejectModal = false">取消</el-button>
            <el-button type="danger" @click="confirmReject">确认拒绝</el-button>
          </div>
        </el-dialog>
      </el-tab-pane>
      
      <!-- 用户管理 -->
      <el-tab-pane label="用户管理" name="users">
        <div class="filter-bar">
          <el-select v-model="userRoleFilter" placeholder="筛选角色">
            <el-option label="全部" value="" />
            <el-option label="管理员" value="admin" />
            <el-option label="农户" value="farmer" />
            <el-option label="消费者" value="consumer" />
          </el-select>
          <el-input v-model="userSearchKeyword" placeholder="搜索用户名" @keyup.enter="filterUsers" style="width: 200px; margin-left: 10px;" />
          <el-button @click="filterUsers">搜索</el-button>
          <el-button type="primary" @click="openAddUserModal" style="margin-left: auto;">添加用户</el-button>
        </div>
        <el-table :data="filteredUsers" border>
          <el-table-column prop="id" label="ID" />
          <el-table-column prop="username" label="用户名" />
          <el-table-column prop="role" label="角色">
            <template #default="scope">
              <el-tag :type="getRoleType(scope.row.role)">{{ getRoleText(scope.row.role) }}</el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="phone" label="手机号" />
          <el-table-column prop="created_at" label="注册时间" />
          <el-table-column label="操作">
            <template #default="scope">
              <el-button size="small" @click="editUser(scope.row)">编辑</el-button>
              <el-button size="small" type="danger" @click="deleteUser(scope.row.id)">删除</el-button>
            </template>
          </el-table-column>
        </el-table>
        
        <!-- 添加用户弹窗 -->
        <el-dialog title="添加用户" v-model="showAddUserModal" width="400px">
          <el-form :model="addUserForm" label-width="80px">
            <el-form-item label="用户名">
              <el-input v-model="addUserForm.username" placeholder="请输入用户名" />
            </el-form-item>
            <el-form-item label="密码">
              <el-input v-model="addUserForm.password" type="password" placeholder="请输入密码" />
            </el-form-item>
            <el-form-item label="手机号">
              <el-input v-model="addUserForm.phone" placeholder="请输入手机号" />
            </el-form-item>
            <el-form-item label="角色">
              <el-select v-model="addUserForm.role">
                <el-option label="管理员" value="admin" />
                <el-option label="农户" value="farmer" />
                <el-option label="消费者" value="consumer" />
              </el-select>
            </el-form-item>
          </el-form>
          <div slot="footer">
            <el-button @click="showAddUserModal = false">取消</el-button>
            <el-button type="primary" @click="addUser">确认添加</el-button>
          </div>
        </el-dialog>
        
        <!-- 编辑用户弹窗 -->
        <el-dialog title="编辑用户" v-model="showEditUserModal" width="400px">
          <el-form :model="editUserForm" label-width="80px">
            <el-form-item label="用户名">
              <el-input v-model="editUserForm.username" />
            </el-form-item>
            <el-form-item label="手机号">
              <el-input v-model="editUserForm.phone" />
            </el-form-item>
            <el-form-item label="角色">
              <el-select v-model="editUserForm.role">
                <el-option label="管理员" value="admin" />
                <el-option label="农户" value="farmer" />
                <el-option label="消费者" value="consumer" />
              </el-select>
            </el-form-item>
          </el-form>
          <div slot="footer">
            <el-button @click="showEditUserModal = false">取消</el-button>
            <el-button type="primary" @click="updateUser">保存修改</el-button>
          </div>
        </el-dialog>
      </el-tab-pane>
      
      <!-- 采购需求管理 -->
      <el-tab-pane label="采购需求" name="purchases">
        <div class="filter-bar">
          <el-select v-model="purchaseStatusFilter" placeholder="筛选状态">
            <el-option label="全部" value="" />
            <el-option label="进行中" value="active" />
            <el-option label="已完成" value="completed" />
          </el-select>
          <el-input v-model="purchaseSearchKeyword" placeholder="搜索商品类型" @keyup.enter="filterPurchases" style="width: 200px; margin-left: 10px;" />
          <el-button @click="filterPurchases">搜索</el-button>
        </div>
        <el-table :data="filteredPurchases" border>
          <el-table-column prop="id" label="ID" />
          <el-table-column prop="product_type" label="需求类型" />
          <el-table-column prop="quantity" label="数量" />
          <el-table-column prop="budget" label="预算(元)" />
          <el-table-column prop="deadline" label="截止日期" />
          <el-table-column prop="status" label="状态">
            <template #default="scope">
              <el-tag :type="scope.row.status === 'active' ? 'primary' : 'success'">
                {{ scope.row.status === 'active' ? '进行中' : '已完成' }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="created_at" label="发布时间" />
          <el-table-column label="操作">
            <template #default="scope">
              <el-button v-if="scope.row.status === 'active'" size="small" type="success" @click="completePurchase(scope.row.id)">标记完成</el-button>
              <el-button size="small" type="danger" @click="deletePurchase(scope.row.id)">删除</el-button>
            </template>
          </el-table-column>
        </el-table>
      </el-tab-pane>
      
      <!-- 评价管理 -->
      <el-tab-pane label="评价管理" name="reviews">
        <div class="filter-bar">
          <el-input v-model="reviewSearchKeyword" placeholder="搜索商品名/用户名" @keyup.enter="filterReviews" style="width: 200px;" />
          <el-button @click="filterReviews">搜索</el-button>
        </div>
        <el-table :data="filteredReviews" border>
          <el-table-column prop="id" label="ID" />
          <el-table-column prop="product_name" label="商品名称" />
          <el-table-column prop="username" label="评价用户" />
          <el-table-column prop="rating" label="评分">
            <template #default="scope">
              <span class="rating-stars">
                <span v-for="i in 5" :key="i" class="star" :class="{ active: i <= scope.row.rating }">★</span>
              </span>
            </template>
          </el-table-column>
          <el-table-column prop="content" label="评价内容" :show-overflow-tooltip="true" />
          <el-table-column prop="created_at" label="评价时间" />
          <el-table-column label="操作">
            <template #default="scope">
              <el-button size="small" type="danger" @click="deleteReview(scope.row.id)">删除</el-button>
            </template>
          </el-table-column>
        </el-table>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, watch } from 'vue'
import axios from 'axios'
import { ElMessage } from 'element-plus'
import { getImageUrl } from '../utils/imageHelper'

const activeTab = ref('stats')

// 商品相关
const allProducts = ref([])
const pendingProducts = ref([])
const productStatusFilter = ref('')
const productSearchKeyword = ref('')
const showProductDetailModal = ref(false)
const selectedProduct = ref(null)
const showRejectModal = ref(false)
const rejectingProductId = ref(null)
const rejectForm = ref({ reason: '' })

// 用户相关
const users = ref([])
const userRoleFilter = ref('')
const userSearchKeyword = ref('')
const showAddUserModal = ref(false)
const showEditUserModal = ref(false)
const editingUserId = ref(null)
const addUserForm = ref({
  username: '',
  password: '',
  phone: '',
  role: 'consumer'
})
const editUserForm = ref({
  username: '',
  phone: '',
  role: 'consumer'
})

// 采购需求相关
const purchases = ref([])
const purchaseStatusFilter = ref('')
const purchaseSearchKeyword = ref('')

// 评价相关
const reviews = ref([])
const reviewSearchKeyword = ref('')

// 统计数据
const farmerCount = ref(0)
const reviewCount = ref(0)
const productCount = ref(0)
const userCount = ref(0)
const demandCount = ref(0)

// 状态类型映射
const getStatusType = (status) => {
  const types = {
    pending: 'warning',
    paid: 'primary',
    shipped: 'info',
    delivered: 'success',
    completed: 'success'
  }
  return types[status] || 'default'
}

const getStatusText = (status) => {
  const texts = {
    pending: '待支付',
    paid: '已支付',
    shipped: '已发货',
    delivered: '已签收',
    completed: '已完成'
  }
  return texts[status] || status
}

const getRoleType = (role) => {
  const types = {
    admin: 'danger',
    farmer: 'success',
    consumer: 'primary'
  }
  return types[role] || 'default'
}

const getRoleText = (role) => {
  const texts = {
    admin: '管理员',
    farmer: '农户',
    consumer: '消费者'
  }
  return texts[role] || role
}

const getRoleIcon = (role) => {
  const icons = {
    admin: '👑',
    farmer: '👨‍🌾',
    consumer: '👤'
  }
  return icons[role] || '👤'
}

const getRoleColor = (role) => {
  const colors = {
    admin: 'linear-gradient(135deg, #8B5E3C 0%, #6B4226 100%)',
    farmer: 'linear-gradient(135deg, #66BB6A 0%, #43A047 100%)',
    consumer: 'linear-gradient(135deg, #87CEEB 0%, #5BA3C8 100%)'
  }
  return colors[role] || '#ddd'
}

const getStatusColor = (status) => {
  const colors = {
    pending: '#F0A500',
    paid: '#87CEEB',
    shipped: '#66BB6A',
    delivered: '#4CAF50',
    completed: '#43A047'
  }
  return colors[status] || '#999'
}

const getProductStatusType = (status) => {
  const types = {
    pending: 'warning',
    approved: 'success',
    rejected: 'danger',
    active: 'primary',
    offline: 'info'
  }
  return types[status] || 'default'
}

const getProductStatusText = (status) => {
  const texts = {
    pending: '待审核',
    approved: '已通过',
    rejected: '已拒绝',
    active: '上架中',
    offline: '已下架'
  }
  return texts[status] || status
}

const getPaymentText = (method) => {
  const texts = {
    'wechat': '微信支付',
    'alipay': '支付宝',
    'bank': '银行卡'
  }
  return texts[method] || method || '未选择'
}

// 计算属性
const filteredProducts = computed(() => {
  let result = allProducts.value
  if (productStatusFilter.value) {
    result = result.filter(p => p.status === productStatusFilter.value)
  }
  if (productSearchKeyword.value) {
    result = result.filter(p => p.name.includes(productSearchKeyword.value))
  }
  return result
})

const filteredUsers = computed(() => {
  let result = users.value
  if (userRoleFilter.value) {
    result = result.filter(u => u.role === userRoleFilter.value)
  }
  if (userSearchKeyword.value) {
    result = result.filter(u => u.username.includes(userSearchKeyword.value))
  }
  return result
})

const filteredPurchases = computed(() => {
  let result = purchases.value
  if (purchaseStatusFilter.value) {
    result = result.filter(p => p.status === purchaseStatusFilter.value)
  }
  if (purchaseSearchKeyword.value) {
    result = result.filter(p => p.product_type.includes(purchaseSearchKeyword.value))
  }
  return result
})

const filteredReviews = computed(() => {
  let result = reviews.value
  if (reviewSearchKeyword.value) {
    const keyword = reviewSearchKeyword.value.toLowerCase()
    result = result.filter(r => 
      (r.product_name && r.product_name.toLowerCase().includes(keyword)) ||
      (r.username && r.username.toLowerCase().includes(keyword))
    )
  }
  return result
})

const productStatusStats = computed(() => {
  const stats = {}
  allProducts.value.forEach(p => {
    stats[p.status] = (stats[p.status] || 0) + 1
  })
  return stats
})

const getProductStatusColor = (status) => {
  const colors = { pending: '#F0A500', approved: '#66BB6A', rejected: '#C0392B', active: '#87CEEB', offline: '#A89880' }
  return colors[status] || '#999'
}

const userRoleStats = computed(() => {
  const stats = {}
  users.value.forEach(u => {
    if (u.role === 'supervisor') return
    stats[u.role] = (stats[u.role] || 0) + 1
  })
  return stats
})

const activeDemandCount = computed(() => {
  return purchases.value.filter(p => p.status === 'active').length
})

const getStatusBarHeight = (count) => {
  const vals = Object.values(productStatusStats.value)
  const max = vals.length > 0 ? Math.max(...vals) : 1
  return (count / max) * 100
}

// 统计卡片跳转到对应管理标签页
const goTab = (tab, filter) => {
  activeTab.value = tab
  if (tab === 'products' && filter === 'pending') {
    productStatusFilter.value = 'pending'
  } else if (tab === 'products') {
    productStatusFilter.value = ''
  }
  if (tab === 'users' && filter === 'farmer') {
    userRoleFilter.value = 'farmer'
  } else if (tab === 'users') {
    userRoleFilter.value = ''
  }
  if (tab === 'purchases' && filter === 'active') {
    purchaseStatusFilter.value = 'active'
  } else if (tab === 'purchases') {
    purchaseStatusFilter.value = ''
  }
}

// 加载数据
const loadData = async () => {
  try {
    const [productsRes, usersRes, purchasesRes, reviewsRes] = await Promise.all([
      axios.get('/admin/products/all'),
      axios.get('/users'),
      axios.get('/purchases'),
      axios.get('/reviews')
    ])

    allProducts.value = productsRes.data || []
    users.value = usersRes.data || []
    purchases.value = purchasesRes.data || []
    reviews.value = reviewsRes.data || []
    pendingProducts.value = allProducts.value.filter(p => p.status === 'pending')
    
    // 更新统计数据
    farmerCount.value = users.value.filter(u => u.role === 'farmer').length
    userCount.value = users.value.length
    reviewCount.value = reviews.value.length
    productCount.value = allProducts.value.length
    demandCount.value = purchases.value.length
    
    // 填充商品的农户名称
    allProducts.value.forEach(p => {
      const farmer = users.value.find(u => u.id === p.farmer_id)
      if (farmer) {
        p.farmer_name = farmer.username
      }
    })
    
  } catch (error) {
    console.error('加载数据失败:', error)
    ElMessage.error('加载数据失败')
  }
}

// 商品审核
const approveProduct = async (id) => {
  try {
    await axios.put(`/admin/products/${id}/approve`)
    ElMessage.success('审核通过')
    loadData()
  } catch (error) {
    ElMessage.error('审核失败')
  }
}

const openRejectModal = (product) => {
  rejectingProductId.value = product.id
  rejectForm.value = { reason: '' }
  showRejectModal.value = true
}

const confirmReject = async () => {
  if (!rejectForm.value.reason.trim()) {
    ElMessage.warning('请输入拒绝原因')
    return
  }
  
  try {
    await axios.put(`/admin/products/${rejectingProductId.value}/reject`)
    ElMessage.success('已拒绝')
    showRejectModal.value = false
    loadData()
  } catch (error) {
    ElMessage.error('拒绝失败')
  }
}

const offlineProduct = async (id) => {
  try {
    await axios.put(`/admin/products/${id}/offline`)
    ElMessage.success('已下架')
    loadData()
  } catch (error) {
    ElMessage.error('下架失败')
  }
}

const viewProductDetail = (product) => {
  selectedProduct.value = product
  showProductDetailModal.value = true
}

const filterProducts = () => {
}

// 用户管理
const openAddUserModal = () => {
  addUserForm.value = {
    username: '',
    password: '',
    phone: '',
    role: 'consumer'
  }
  showAddUserModal.value = true
}

const addUser = async () => {
  if (!addUserForm.value.username || !addUserForm.value.password) {
    ElMessage.warning('请填写用户名和密码')
    return
  }
  
  try {
    await axios.post('/register', addUserForm.value)
    ElMessage.success('用户添加成功')
    showAddUserModal.value = false
    loadData()
  } catch (error) {
    ElMessage.error(error.response?.data?.error || '添加失败')
  }
}

const editUser = (user) => {
  editingUserId.value = user.id
  editUserForm.value = {
    username: user.username,
    phone: user.phone,
    role: user.role
  }
  showEditUserModal.value = true
}

const updateUser = async () => {
  try {
    await axios.put(`/users/${editingUserId.value}`, editUserForm.value)
    ElMessage.success('用户信息已更新')
    showEditUserModal.value = false
    loadData()
  } catch (error) {
    ElMessage.error('更新失败')
  }
}

const deleteUser = async (id) => {
  try {
    await axios.delete(`/users/${id}`)
    ElMessage.success('用户已删除')
    loadData()
  } catch (error) {
    ElMessage.error('删除失败')
  }
}

const filterUsers = () => {
}

// 采购需求管理
const completePurchase = async (id) => {
  try {
    await axios.put(`/purchases/${id}`, { status: 'completed' })
    ElMessage.success('已标记完成')
    loadData()
  } catch (error) {
    ElMessage.error('操作失败')
  }
}

const deletePurchase = async (id) => {
  try {
    await axios.delete(`/purchases/${id}`)
    ElMessage.success('已删除')
    loadData()
  } catch (error) {
    ElMessage.error('删除失败')
  }
}

const filterPurchases = () => {
}

// 评价管理
const deleteReview = async (id) => {
  try {
    await axios.delete(`/reviews/${id}`)
    ElMessage.success('评价已删除')
    loadData()
  } catch (error) {
    ElMessage.error('删除失败')
  }
}

const filterReviews = () => {
}

// 切换标签页时自动刷新数据，确保评价等数据实时同步
watch(activeTab, () => {
  loadData()
})

onMounted(() => {
  loadData()
})
</script>

<style scoped>
.admin-container {
  padding: 24px;
}

/* === 统计卡片（基础）=== */
.stat-card {
  padding: 24px 20px;
  text-align: center;
  color: white;
  border-radius: var(--radius-md);
  position: relative;
  overflow: hidden;
  transition: transform var(--transition-normal);
}
.stat-card:hover {
  transform: translateY(-4px);
}

/* 统计卡片 — 农业主题色变体 */
.stat-card-green {
  background: linear-gradient(135deg, #66BB6A 0%, #43A047 100%);
}
.stat-card-gold {
  background: linear-gradient(135deg, var(--color-gold) 0%, var(--color-gold-dark) 100%);
}
.stat-card-earth {
  background: linear-gradient(135deg, var(--color-earth-light) 0%, var(--color-earth) 100%);
}
.stat-card-green-light {
  background: linear-gradient(135deg, #81C784 0%, #66BB6A 100%);
}
.stat-card-warm {
  background: linear-gradient(135deg, #C8956C 0%, #F4A460 100%);
}
.stat-card-sky {
  background: linear-gradient(135deg, #87CEEB 0%, #5BA3C8 100%);
}
.stat-card-soil {
  background: linear-gradient(135deg, #8B5E3C 0%, #A0785A 100%);
}
.stat-card-earth-light {
  background: linear-gradient(135deg, #D4A574 0%, #C8956C 100%);
}

.stat-icon {
  font-size: 24px;
  position: absolute;
  top: 12px;
  right: 16px;
  opacity: 0.7;
}

.stat-num {
  font-size: 32px;
  font-weight: bold;
  margin-top: 8px;
}

.stat-text {
  font-size: 14px;
  opacity: 0.85;
}

.stat-hint {
  font-size: 11px;
  opacity: 0;
  margin-top: 6px;
  transition: opacity var(--transition-fast);
}
.stat-card:hover .stat-hint {
  opacity: 0.9;
}

/* === 图表卡片 === */
.chart-card {
  background: var(--bg-card);
  padding: 24px;
  border-radius: var(--radius-md);
  box-shadow: var(--shadow-card);
  border: 1px solid var(--color-primary-bg);
}

.chart-card h4 {
  margin-bottom: 20px;
  color: var(--text-primary);
  font-size: 16px;
}

/* 订单状态分布 */
.status-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  height: 100px;
}

.status-bar {
  width: 30px;
  border-radius: var(--radius-sm);
  transition: height 0.3s;
  min-height: 10px;
}

.status-label {
  font-size: 12px;
  color: var(--text-secondary);
  margin-top: 8px;
}

.status-count {
  font-size: 14px;
  font-weight: bold;
  color: var(--text-primary);
}

/* 用户角色分布 */
.role-item {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.role-circle {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 24px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.15);
}

.role-label {
  font-size: 12px;
  color: var(--text-secondary);
  margin-top: 8px;
}

.role-count {
  font-size: 16px;
  font-weight: bold;
  color: var(--text-primary);
}

/* === 筛选栏 === */
.filter-bar {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 20px;
}

/* === 详情弹窗 === */
.product-detail, .order-detail {
  padding: 10px;
}

.detail-img {
  width: 100%;
  height: 200px;
  object-fit: cover;
  border-radius: var(--radius-sm);
}

.detail-item {
  margin-bottom: 10px;
}

.detail-label {
  color: var(--text-muted);
  font-size: 14px;
}

.detail-value {
  color: var(--text-primary);
  font-size: 14px;
}

.detail-value.total-price {
  color: var(--text-price);
  font-size: 18px;
  font-weight: bold;
}

.detail-section {
  margin-bottom: 15px;
}

.detail-section h4 {
  margin-bottom: 12px;
  color: var(--color-primary);
  font-size: 14px;
}

/* === 评分星星 === */
.rating-stars {
  font-size: 14px;
}

.star {
  color: var(--star-inactive);
}

.star.active {
  color: var(--star-color);
}
</style>