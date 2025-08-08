<template>
  <form @submit.prevent="handleSubmit" class="space-y-6">
    <!-- Basic Information -->
    <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Title *</label>
        <input
          v-model="form.title"
          type="text"
          required
          class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
          placeholder="Enter ticket title"
        />
      </div>
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Customer *</label>
        <select
          v-model="form.counterpartyId"
          required
          class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
        >
          <option value="">Select Customer</option>
          <option
            v-for="counterparty in customers"
            :key="counterparty.id"
            :value="counterparty.id"
          >
            {{ counterparty.name }}
          </option>
        </select>
      </div>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Amount *</label>
        <input
          v-model.number="form.amount"
          type="number"
          step="0.01"
          required
          class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
          placeholder="0.00"
        />
      </div>
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Currency *</label>
        <select
          v-model="form.currency"
          required
          class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
        >
          <option value="IRR">IRR</option>
          <option value="USD">USD</option>
          <option value="EUR">EUR</option>
        </select>
      </div>
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Type *</label>
        <select
          v-model="form.type"
          required
          class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
        >
          <option value="0">Domestic</option>
          <option value="1">International</option>
        </select>
      </div>
    </div>

    <div>
      <label class="block text-sm font-medium text-gray-700 mb-1">Description</label>
      <textarea
        v-model="form.description"
        rows="3"
        class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
        placeholder="Enter ticket description"
      ></textarea>
    </div>

    <!-- Ticket Items -->
    <div>
      <div class="flex justify-between items-center mb-4">
        <h3 class="text-lg font-medium text-gray-900">Ticket Items</h3>
        <button
          type="button"
          @click="addTicketItem"
          class="bg-green-600 hover:bg-green-700 text-white px-3 py-1 rounded text-sm flex items-center gap-1"
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path>
          </svg>
          Add Item
        </button>
      </div>

      <div class="space-y-4">
        <div
          v-for="(item, index) in form.items"
          :key="index"
          class="border border-gray-200 rounded-lg p-4"
        >
          <div class="flex justify-between items-start mb-4">
            <h4 class="font-medium text-gray-900">Item {{ index + 1 }}</h4>
            <button
              type="button"
              @click="removeTicketItem(index)"
              class="text-red-600 hover:text-red-800"
            >
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path>
              </svg>
            </button>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Passenger Name *</label>
              <input
                v-model="item.passengerName"
                type="text"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="Enter passenger name"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Airline *</label>
              <select
                v-model="item.airlineId"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="">Select Airline</option>
                <option
                  v-for="airline in airlines"
                  :key="airline.id"
                  :value="airline.id"
                >
                  {{ airline.name }}
                </option>
              </select>
            </div>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mt-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Origin *</label>
              <select
                v-model="item.originId"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="">Select Origin</option>
                <option
                  v-for="origin in origins"
                  :key="origin.id"
                  :value="origin.id"
                >
                  {{ origin.name }} ({{ origin.code }})
                </option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Destination *</label>
              <select
                v-model="item.destinationId"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="">Select Destination</option>
                <option
                  v-for="destination in destinations"
                  :key="destination.id"
                  :value="destination.id"
                >
                  {{ destination.name }} ({{ destination.code }})
                </option>
              </select>
            </div>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mt-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Service Date *</label>
              <input
                v-model="item.serviceDate"
                type="date"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Flight Number</label>
              <input
                v-model="item.flightNumber"
                type="text"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="Enter flight number"
              />
            </div>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mt-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Amount *</label>
              <input
                v-model.number="item.amount"
                type="number"
                step="0.01"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="0.00"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Currency *</label>
              <select
                v-model="item.currency"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="IRR">IRR</option>
                <option value="USD">USD</option>
                <option value="EUR">EUR</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Tax Amount</label>
              <input
                v-model.number="item.taxAmount"
                type="number"
                step="0.01"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="0.00"
              />
            </div>
          </div>

          <div class="mt-4">
            <label class="block text-sm font-medium text-gray-700 mb-1">Notes</label>
            <textarea
              v-model="item.notes"
              rows="2"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Enter item notes"
            ></textarea>
          </div>
        </div>
      </div>
    </div>

    <!-- Form Actions -->
    <div class="flex justify-end space-x-3 pt-6 border-t">
      <button
        type="button"
        @click="$emit('cancel')"
        class="px-4 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500"
      >
        Cancel
      </button>
      <button
        type="submit"
        :disabled="loading"
        class="px-4 py-2 bg-blue-600 border border-transparent rounded-md text-sm font-medium text-white hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:opacity-50"
      >
        {{ loading ? 'Saving...' : (isEdit ? 'Update Ticket' : 'Create Ticket') }}
      </button>
    </div>
  </form>
</template>

<script>
import { ref, reactive, onMounted, watch } from 'vue'
import { useCounterpartyStore } from '@/stores/counterpartyStore'
import { useAirlineStore } from '@/stores/airlineStore'
import { useOriginStore } from '@/stores/originStore'
import { useDestinationStore } from '@/stores/destinationStore'

export default {
  name: 'TicketForm',
  props: {
    ticket: {
      type: Object,
      default: null
    },
    isEdit: {
      type: Boolean,
      default: false
    }
  },
  emits: ['save', 'cancel'],
  setup(props, { emit }) {
    const counterpartyStore = useCounterpartyStore()
    const airlineStore = useAirlineStore()
    const originStore = useOriginStore()
    const destinationStore = useDestinationStore()

    const loading = ref(false)
    const customers = ref([])
    const airlines = ref([])
    const origins = ref([])
    const destinations = ref([])

    const form = reactive({
      title: '',
      counterpartyId: '',
      amount: 0,
      currency: 'IRR',
      type: '0',
      description: '',
      items: []
    })

    const createEmptyItem = () => ({
      passengerName: '',
      airlineId: '',
      originId: '',
      destinationId: '',
      serviceDate: '',
      flightNumber: '',
      amount: 0,
      currency: 'IRR',
      taxAmount: 0,
      notes: ''
    })

    const addTicketItem = () => {
      form.items.push(createEmptyItem())
    }

    const removeTicketItem = (index) => {
      form.items.splice(index, 1)
    }

    const loadMasterData = async () => {
      try {
        // Load customers (counterparties that are customers)
        const counterpartiesResult = await counterpartyStore.getCounterparties({
          isCustomer: true,
          isActive: true,
          pageSize: 1000
        })
        customers.value = counterpartiesResult.items

        // Load airlines
        const airlinesResult = await airlineStore.getAirlines({
          isActive: true,
          pageSize: 1000
        })
        airlines.value = airlinesResult.items

        // Load origins
        const originsResult = await originStore.getOrigins({
          isActive: true,
          pageSize: 1000
        })
        origins.value = originsResult.items

        // Load destinations
        const destinationsResult = await destinationStore.getDestinations({
          isActive: true,
          pageSize: 1000
        })
        destinations.value = destinationsResult.items
      } catch (error) {
        console.error('Error loading master data:', error)
      }
    }

    const populateForm = () => {
      if (props.ticket) {
        form.title = props.ticket.title || ''
        form.counterpartyId = props.ticket.counterpartyId || ''
        form.amount = props.ticket.amount || 0
        form.currency = props.ticket.currency || 'IRR'
        form.type = props.ticket.type?.toString() || '0'
        form.description = props.ticket.description || ''
        
        if (props.ticket.items && props.ticket.items.length > 0) {
          form.items = props.ticket.items.map(item => ({
            id: item.id,
            passengerName: item.passengerName || '',
            airlineId: item.airlineId || '',
            originId: item.originId || '',
            destinationId: item.destinationId || '',
            serviceDate: item.serviceDate ? item.serviceDate.split('T')[0] : '',
            flightNumber: item.flightNumber || '',
            amount: item.amount || 0,
            currency: item.currency || 'IRR',
            taxAmount: item.taxAmount || 0,
            notes: item.notes || ''
          }))
        } else {
          form.items = [createEmptyItem()]
        }
      } else {
        // Reset form for new ticket
        Object.assign(form, {
          title: '',
          counterpartyId: '',
          amount: 0,
          currency: 'IRR',
          type: '0',
          description: '',
          items: [createEmptyItem()]
        })
      }
    }

    const handleSubmit = async () => {
      loading.value = true
      try {
        const ticketData = {
          title: form.title,
          counterpartyId: form.counterpartyId,
          amount: form.amount,
          currency: form.currency,
          type: parseInt(form.type),
          description: form.description,
          items: form.items.map(item => ({
            id: item.id,
            passengerName: item.passengerName,
            airlineId: item.airlineId,
            originId: item.originId,
            destinationId: item.destinationId,
            serviceDate: item.serviceDate,
            flightNumber: item.flightNumber,
            amount: item.amount,
            currency: item.currency,
            taxAmount: item.taxAmount,
            notes: item.notes
          }))
        }

        emit('save', ticketData)
      } catch (error) {
        console.error('Error submitting form:', error)
      } finally {
        loading.value = false
      }
    }

    watch(() => props.ticket, () => {
      populateForm()
    }, { immediate: true })

    onMounted(() => {
      loadMasterData()
      populateForm()
    })

    return {
      form,
      loading,
      customers,
      airlines,
      origins,
      destinations,
      addTicketItem,
      removeTicketItem,
      handleSubmit
    }
  }
}
</script>