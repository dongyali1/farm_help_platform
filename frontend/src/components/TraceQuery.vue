<template>
  <div class="trace-container">
    <div class="query-section">
      <h2>📦 农产品溯源查询</h2>
      <el-input v-model="traceCode" placeholder="请输入溯源码" class="trace-input">
        <template #append>
          <el-button type="primary" @click="queryTrace">查询</el-button>
        </template>
      </el-input>
    </div>
    
    <div v-if="traceInfo.length > 0" class="result-section">
      <h3>溯源信息</h3>
      <div class="timeline">
        <div v-for="(item, index) in traceInfo" :key="index" class="timeline-item">
          <div class="timeline-dot"></div>
          <div class="timeline-content">
            <div class="timeline-header">
              <span class="node-type">{{ getNodeTypeName(item.node_type) }}</span>
              <span class="node-time">{{ item.time }}</span>
            </div>
            <p class="node-location">📍 {{ item.location }}</p>
            <p class="node-desc">{{ item.description }}</p>
            <p class="node-operator">操作人: {{ item.operator }}</p>
          </div>
        </div>
      </div>
    </div>
    
    <div v-else-if="searched && traceInfo.length === 0" class="empty-section">
      <p>未找到溯源信息，请检查溯源码是否正确</p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import axios from 'axios'
import { ElMessage } from 'element-plus'

const traceCode = ref('')
const traceInfo = ref([])
const searched = ref(false)

const getNodeTypeName = (type) => {
  const types = {
    planting: '种植/养殖',
    processing: '加工',
    logistics: '物流运输',
    storage: '仓储',
    inspection: '质检'
  }
  return types[type] || type
}

const queryTrace = async () => {
  if (!traceCode.value) {
    ElMessage.warning('请输入溯源码')
    return
  }
  
  try {
    const response = await axios.get(`/trace/${traceCode.value}`)
    traceInfo.value = response.data
    searched.value = true
  } catch (error) {
    ElMessage.error(error.response?.data?.error || '查询失败')
    traceInfo.value = []
    searched.value = true
  }
}
</script>

<style scoped>
.trace-container {
  padding: 24px;
}
.query-section {
  text-align: center;
  margin-bottom: 36px;
}
.query-section h2 {
  color: var(--color-primary);
  margin-bottom: 20px;
}
.trace-input {
  width: 400px;
}
.result-section {
  max-width: 800px;
  margin: 0 auto;
}
.result-section h3 {
  color: var(--text-primary);
  margin-bottom: 20px;
}
.timeline {
  position: relative;
  padding-left: 30px;
}
.timeline::before {
  content: '';
  position: absolute;
  left: 8px;
  top: 0;
  bottom: 0;
  width: 2px;
  background: var(--color-primary);
}
.timeline-item {
  position: relative;
  margin-bottom: 30px;
}
.timeline-dot {
  position: absolute;
  left: -26px;
  top: 5px;
  width: 16px;
  height: 16px;
  background: var(--color-primary);
  border-radius: 50%;
  box-shadow: 0 0 0 4px var(--color-primary-bg);
}
.timeline-content {
  background: var(--bg-grey);
  padding: 20px;
  border-radius: var(--radius-md);
  box-shadow: var(--shadow-card);
}
.timeline-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 12px;
}
.node-type {
  background: var(--color-primary);
  color: white;
  padding: 5px 16px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 500;
}
.node-time {
  color: var(--text-muted);
  font-size: 12px;
}
.node-location {
  font-weight: bold;
  color: var(--text-primary);
}
.node-desc {
  color: var(--text-secondary);
  line-height: 1.5;
}
.node-operator {
  color: var(--text-muted);
  font-size: 12px;
  margin-top: 10px;
}
.empty-section {
  text-align: center;
  padding: 50px;
  color: var(--text-muted);
}
</style>