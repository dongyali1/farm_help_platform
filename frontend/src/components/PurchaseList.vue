<template>
  <div class="purchase-container">
    <div class="header">
      <h2>📋 需求广场</h2>
      <el-button type="primary" @click="showAddModal = true">发布采购需求</el-button>
    </div>

    <el-table :data="purchases" border>
      <el-table-column prop="id" label="ID" width="60" />
      <el-table-column prop="product_type" label="需求类型" />
      <el-table-column prop="quantity" label="数量(kg)" width="100" />
      <el-table-column prop="budget" label="预算(元)" width="100" />
      <el-table-column prop="deadline" label="截止日期" width="120" />
      <el-table-column prop="contact_name" label="联系人" width="100" />
      <el-table-column prop="contact_phone" label="联系电话" width="130">
        <template #default="scope">
          <span class="contact-phone-link">📱 {{ scope.row.contact_phone }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="status" label="状态" width="90">
        <template #default="scope">
          <el-tag :type="scope.row.status === 'active' ? 'primary' : 'success'">
            {{ scope.row.status === 'active' ? '进行中' : '已完成' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="created_at" label="发布时间" width="170" />
      <el-table-column label="操作" width="140" fixed="right">
        <template #default="scope">
          <el-button v-if="scope.row.status === 'active'" size="small" type="success" @click="completeDemand(scope.row.id)">标记完成</el-button>
          <el-button size="small" type="danger" @click="deleteDemand(scope.row.id)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <!-- 发布需求弹窗 -->
    <el-dialog title="发布采购需求" v-model="showAddModal" width="450px">
      <el-form :model="purchaseForm" label-width="80px">
        <el-form-item label="需求类型" required>
          <el-select v-model="purchaseForm.product_type" placeholder="请选择农产品类型" style="width: 100%;">
            <el-option label="蔬菜水果" value="蔬菜水果" />
            <el-option label="粮油米面" value="粮油米面" />
            <el-option label="肉类禽蛋" value="肉类禽蛋" />
            <el-option label="特色农产品" value="特色农产品" />
          </el-select>
        </el-form-item>
        <el-form-item label="需求数量(kg)" required>
          <el-input-number v-model="purchaseForm.quantity" :min="1" style="width: 100%;" placeholder="单位：kg" />
        </el-form-item>
        <el-form-item label="预算(元)">
          <el-input v-model="purchaseForm.budget" type="number" placeholder="可接受的预算范围" />
        </el-form-item>
        <el-form-item label="截止日期">
          <el-date-picker v-model="purchaseForm.deadline" type="date" placeholder="选择截止日期" style="width: 100%;" />
        </el-form-item>
        <el-divider content-position="left">📞 联系方式（农户可见）</el-divider>
        <el-form-item label="联系人" required>
          <el-input v-model="purchaseForm.contact_name" placeholder="您的姓名" />
        </el-form-item>
        <el-form-item label="联系电话" required>
          <el-input v-model="purchaseForm.contact_phone" placeholder="您的手机号码" />
        </el-form-item>
      </el-form>
      <div slot="footer">
        <el-button @click="showAddModal = false">取消</el-button>
        <el-button type="primary" @click="submitPurchase">发布需求</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import { ElMessage } from 'element-plus'

const props = defineProps({
  currentUser: { type: Object, default: null }
})

const purchases = ref([])
const showAddModal = ref(false)

const purchaseForm = ref({
  product_type: '',
  quantity: 1,
  budget: null,
  deadline: '',
  contact_name: '',
  contact_phone: ''
})

const getPurchases = async () => {
  try {
    const response = await axios.get('/purchases')
    purchases.value = response.data
  } catch (error) {
    ElMessage.error('获取需求列表失败')
  }
}

const submitPurchase = async () => {
  if (!purchaseForm.value.product_type) {
    ElMessage.warning('请选择需求类型')
    return
  }
  if (!purchaseForm.value.contact_name || !purchaseForm.value.contact_phone) {
    ElMessage.warning('请填写联系方式')
    return
  }
  try {
    await axios.post('/purchases', {
      buyer_id: props.currentUser?.id || 1,
      product_type: purchaseForm.value.product_type,
      quantity: purchaseForm.value.quantity,
      budget: purchaseForm.value.budget,
      deadline: purchaseForm.value.deadline,
      contact_name: purchaseForm.value.contact_name,
      contact_phone: purchaseForm.value.contact_phone
    })
    ElMessage.success('需求发布成功！农户将能看到您的联系方式')
    showAddModal.value = false
    getPurchases()
  } catch (error) {
    ElMessage.error(error.response?.data?.error || '发布失败')
  }
}

const completeDemand = async (id) => {
  try {
    await axios.put(`/purchases/${id}`, { status: 'completed' })
    ElMessage.success('已标记完成')
    getPurchases()
  } catch (error) {
    ElMessage.error('操作失败')
  }
}

const deleteDemand = async (id) => {
  try {
    await axios.delete(`/purchases/${id}`)
    ElMessage.success('已删除')
    getPurchases()
  } catch (error) {
    ElMessage.error('删除失败')
  }
}

onMounted(() => { getPurchases() })
</script>

<style scoped>
.purchase-container { padding: 24px; }
.purchase-container h2 { color: var(--color-primary); }
.header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; }
.contact-phone-link { color: var(--color-primary); font-weight: 500; }
</style>
