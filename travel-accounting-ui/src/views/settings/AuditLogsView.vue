<template>
  <div class="audit-logs-view">
    <div class="header">
      <h1>Audit Logs</h1>
      <p class="subtitle">Track system activities and changes</p>
    </div>

    <!-- Filters -->
    <div class="filters-card">
      <div class="filters-grid">
        <div class="filter-group">
          <label>Entity Name</label>
          <input
            v-model="filters.entityName"
            type="text"
            placeholder="Filter by entity..."
            class="filter-input"
          />
        </div>
        
        <div class="filter-group">
          <label>Action</label>
          <select v-model="filters.action" class="filter-select">
            <option value="">All Actions</option>
            <option value="Create">Create</option>
            <option value="Update">Update</option>
            <option value="Delete">Delete</option>
            <option value="Login">Login</option>
            <option value="Logout">Logout</option>
            <option value="Approve">Approve</option>
            <option value="Reject">Reject</option>
          </select>
        </div>

        <div class="filter-group">
          <label>Start Date</label>
          <input
            v-model="filters.startDate"
            type="date"
            class="filter-input"
          />
        </div>

        <div class="filter-group">
          <label>End Date</label>
          <input
            v-model="filters.endDate"
            type="date"
            class="filter-input"
          />
        </div>

        <div class="filter-group">
          <label>Search</label>
          <input
            v-model="filters.searchTerm"
            type="text"
            placeholder="Search in changes..."
            class="filter-input"
          />
        </div>

        <div class="filter-actions">
          <button @click="applyFilters" class="btn btn-primary">
            <i class="fas fa-search"></i>
            Apply Filters
          </button>
          <button @click="clearFilters" class="btn btn-secondary">
            <i class="fas fa-times"></i>
            Clear
          </button>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="auditStore.loading" class="loading">
      <i class="fas fa-spinner fa-spin"></i>
      Loading audit logs...
    </div>

    <!-- Error State -->
    <div v-if="auditStore.error" class="error-message">
      <i class="fas fa-exclamation-triangle"></i>
      {{ auditStore.error }}
    </div>

    <!-- Audit Logs Table -->
    <div v-if="!auditStore.loading && !auditStore.error" class="table-card">
      <div class="table-header">
        <h3>Audit Trail</h3>
        <div class="table-info">
          Showing {{ auditStore.auditLogs.length }} of {{ auditStore.totalCount }} entries
        </div>
      </div>

      <div class="table-container">
        <table class="audit-table">
          <thead>
            <tr>
              <th>Timestamp</th>
              <th>Entity</th>
              <th>Action</th>
              <th>User</th>
              <th>Changes</th>
              <th>IP Address</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="log in auditStore.auditLogs" :key="log.id">
              <td>
                <div class="timestamp">
                  {{ formatDateTime(log.timestamp) }}
                </div>
              </td>
              <td>
                <div class="entity-info">
                  <div class="entity-name">{{ log.entityName }}</div>
                  <div v-if="log.entityId" class="entity-id">ID: {{ log.entityId }}</div>
                </div>
              </td>
              <td>
                <span :class="['action-badge', `action-${log.action.toLowerCase()}`]">
                  {{ log.actionName }}
                </span>
              </td>
              <td>
                <div class="user-info">
                  {{ log.userName || 'System' }}
                </div>
              </td>
              <td>
                <div class="changes-preview">
                  {{ truncateText(log.changes, 50) }}
                </div>
              </td>
              <td>
                <div class="ip-address">
                  {{ log.ipAddress }}
                </div>
              </td>
              <td>
                <button
                  @click="viewDetails(log)"
                  class="btn btn-sm btn-outline"
                  title="View Details"
                >
                  <i class="fas fa-eye"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div class="pagination">
        <button
          @click="previousPage"
          :disabled="currentPage === 1"
          class="btn btn-secondary"
        >
          <i class="fas fa-chevron-left"></i>
          Previous
        </button>
        
        <span class="page-info">
          Page {{ currentPage }} of {{ totalPages }}
        </span>
        
        <button
          @click="nextPage"
          :disabled="currentPage === totalPages"
          class="btn btn-secondary"
        >
          Next
          <i class="fas fa-chevron-right"></i>
        </button>
      </div>
    </div>

    <!-- Details Modal -->
    <div v-if="showDetailsModal" class="modal-overlay" @click="closeDetailsModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>Audit Log Details</h3>
          <button @click="closeDetailsModal" class="btn btn-close">
            <i class="fas fa-times"></i>
          </button>
        </div>
        
        <div class="modal-body">
          <div v-if="selectedLog" class="details-grid">
            <div class="detail-item">
              <label>Timestamp:</label>
              <span>{{ formatDateTime(selectedLog.timestamp) }}</span>
            </div>
            
            <div class="detail-item">
              <label>Entity:</label>
              <span>{{ selectedLog.entityName }} (ID: {{ selectedLog.entityId || 'N/A' }})</span>
            </div>
            
            <div class="detail-item">
              <label>Action:</label>
              <span :class="['action-badge', `action-${selectedLog.action.toLowerCase()}`]">
                {{ selectedLog.actionName }}
              </span>
            </div>
            
            <div class="detail-item">
              <label>User:</label>
              <span>{{ selectedLog.userName || 'System' }}</span>
            </div>
            
            <div class="detail-item">
              <label>IP Address:</label>
              <span>{{ selectedLog.ipAddress }}</span>
            </div>
            
            <div class="detail-item">
              <label>User Agent:</label>
              <span>{{ selectedLog.userAgent || 'N/A' }}</span>
            </div>
            
            <div class="detail-item full-width">
              <label>Changes:</label>
              <pre class="changes-content">{{ selectedLog.changes }}</pre>
            </div>
            
            <div v-if="selectedLog.oldValues" class="detail-item full-width">
              <label>Old Values:</label>
              <pre class="json-content">{{ formatJson(selectedLog.oldValues) }}</pre>
            </div>
            
            <div v-if="selectedLog.newValues" class="detail-item full-width">
              <label>New Values:</label>
              <pre class="json-content">{{ formatJson(selectedLog.newValues) }}</pre>
            </div>
            
            <div v-if="selectedLog.additionalInfo" class="detail-item full-width">
              <label>Additional Info:</label>
              <pre class="additional-info">{{ selectedLog.additionalInfo }}</pre>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { useAuditStore } from '@/stores/auditStore'

export default {
  name: 'AuditLogsView',
  setup() {
    const auditStore = useAuditStore()
    
    const filters = reactive({
      entityName: '',
      action: '',
      startDate: '',
      endDate: '',
      searchTerm: ''
    })
    
    const currentPage = ref(1)
    const pageSize = ref(20)
    const showDetailsModal = ref(false)
    const selectedLog = ref(null)
    
    const totalPages = computed(() => {
      return Math.ceil(auditStore.totalCount / pageSize.value)
    })
    
    const loadAuditLogs = async () => {
      const query = {
        page: currentPage.value,
        pageSize: pageSize.value,
        ...filters
      }
      
      // Remove empty filters
      Object.keys(query).forEach(key => {
        if (query[key] === '' || query[key] === null) {
          delete query[key]
        }
      })
      
      await auditStore.getAuditLogs(query)
    }
    
    const applyFilters = () => {
      currentPage.value = 1
      loadAuditLogs()
    }
    
    const clearFilters = () => {
      Object.keys(filters).forEach(key => {
        filters[key] = ''
      })
      currentPage.value = 1
      loadAuditLogs()
    }
    
    const previousPage = () => {
      if (currentPage.value > 1) {
        currentPage.value--
        loadAuditLogs()
      }
    }
    
    const nextPage = () => {
      if (currentPage.value < totalPages.value) {
        currentPage.value++
        loadAuditLogs()
      }
    }
    
    const viewDetails = (log) => {
      selectedLog.value = log
      showDetailsModal.value = true
    }
    
    const closeDetailsModal = () => {
      showDetailsModal.value = false
      selectedLog.value = null
    }
    
    const formatDateTime = (dateString) => {
      return new Date(dateString).toLocaleString()
    }
    
    const truncateText = (text, maxLength) => {
      if (!text) return ''
      return text.length > maxLength ? text.substring(0, maxLength) + '...' : text
    }
    
    const formatJson = (jsonString) => {
      try {
        return JSON.stringify(JSON.parse(jsonString), null, 2)
      } catch {
        return jsonString
      }
    }
    
    onMounted(() => {
      loadAuditLogs()
    })
    
    return {
      auditStore,
      filters,
      currentPage,
      totalPages,
      showDetailsModal,
      selectedLog,
      loadAuditLogs,
      applyFilters,
      clearFilters,
      previousPage,
      nextPage,
      viewDetails,
      closeDetailsModal,
      formatDateTime,
      truncateText,
      formatJson
    }
  }
}
</script>

<style scoped>
.audit-logs-view {
  padding: 2rem;
  max-width: 1400px;
  margin: 0 auto;
}

.header {
  margin-bottom: 2rem;
}

.header h1 {
  color: #2c3e50;
  margin-bottom: 0.5rem;
}

.subtitle {
  color: #7f8c8d;
  font-size: 1.1rem;
}

.filters-card {
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  margin-bottom: 2rem;
}

.filters-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
  align-items: end;
}

.filter-group {
  display: flex;
  flex-direction: column;
}

.filter-group label {
  font-weight: 600;
  margin-bottom: 0.5rem;
  color: #2c3e50;
}

.filter-input,
.filter-select {
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 0.9rem;
}

.filter-actions {
  display: flex;
  gap: 0.5rem;
}

.table-card {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.table-header {
  padding: 1.5rem;
  border-bottom: 1px solid #eee;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.table-header h3 {
  margin: 0;
  color: #2c3e50;
}

.table-info {
  color: #7f8c8d;
  font-size: 0.9rem;
}

.table-container {
  overflow-x: auto;
}

.audit-table {
  width: 100%;
  border-collapse: collapse;
}

.audit-table th {
  background: #f8f9fa;
  padding: 1rem;
  text-align: left;
  font-weight: 600;
  color: #2c3e50;
  border-bottom: 1px solid #eee;
}

.audit-table td {
  padding: 1rem;
  border-bottom: 1px solid #f0f0f0;
  vertical-align: top;
}

.timestamp {
  font-size: 0.9rem;
  color: #666;
}

.entity-info {
  display: flex;
  flex-direction: column;
}

.entity-name {
  font-weight: 600;
  color: #2c3e50;
}

.entity-id {
  font-size: 0.8rem;
  color: #7f8c8d;
}

.action-badge {
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 600;
  text-transform: uppercase;
}

.action-create {
  background: #d4edda;
  color: #155724;
}

.action-update {
  background: #fff3cd;
  color: #856404;
}

.action-delete {
  background: #f8d7da;
  color: #721c24;
}

.action-login,
.action-logout {
  background: #cce5ff;
  color: #004085;
}

.action-approve {
  background: #d1ecf1;
  color: #0c5460;
}

.action-reject {
  background: #f5c6cb;
  color: #721c24;
}

.user-info {
  font-weight: 500;
  color: #2c3e50;
}

.changes-preview {
  font-size: 0.9rem;
  color: #666;
  max-width: 200px;
}

.ip-address {
  font-family: monospace;
  font-size: 0.9rem;
  color: #666;
}

.pagination {
  padding: 1.5rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-top: 1px solid #eee;
}

.page-info {
  color: #666;
  font-weight: 500;
}

.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 500;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  transition: all 0.2s;
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-primary {
  background: #007bff;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background: #0056b3;
}

.btn-secondary {
  background: #6c757d;
  color: white;
}

.btn-secondary:hover:not(:disabled) {
  background: #545b62;
}

.btn-outline {
  background: transparent;
  border: 1px solid #ddd;
  color: #666;
}

.btn-outline:hover {
  background: #f8f9fa;
}

.btn-sm {
  padding: 0.25rem 0.5rem;
  font-size: 0.8rem;
}

.btn-close {
  background: transparent;
  border: none;
  font-size: 1.2rem;
  color: #666;
  padding: 0.25rem;
}

.loading,
.error-message {
  text-align: center;
  padding: 3rem;
  color: #666;
}

.error-message {
  color: #dc3545;
  background: #f8d7da;
  border-radius: 4px;
}

.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 8px;
  max-width: 800px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
}

.modal-header {
  padding: 1.5rem;
  border-bottom: 1px solid #eee;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal-header h3 {
  margin: 0;
  color: #2c3e50;
}

.modal-body {
  padding: 1.5rem;
}

.details-grid {
  display: grid;
  grid-template-columns: 1fr 2fr;
  gap: 1rem;
}

.detail-item {
  display: flex;
  flex-direction: column;
}

.detail-item.full-width {
  grid-column: 1 / -1;
}

.detail-item label {
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 0.5rem;
}

.changes-content,
.json-content,
.additional-info {
  background: #f8f9fa;
  padding: 1rem;
  border-radius: 4px;
  font-family: monospace;
  font-size: 0.9rem;
  white-space: pre-wrap;
  word-break: break-word;
  max-height: 200px;
  overflow-y: auto;
}

.json-content {
  background: #f0f8ff;
}

@media (max-width: 768px) {
  .audit-logs-view {
    padding: 1rem;
  }
  
  .filters-grid {
    grid-template-columns: 1fr;
  }
  
  .table-container {
    font-size: 0.8rem;
  }
  
  .pagination {
    flex-direction: column;
    gap: 1rem;
  }
  
  .details-grid {
    grid-template-columns: 1fr;
  }
}
</style>