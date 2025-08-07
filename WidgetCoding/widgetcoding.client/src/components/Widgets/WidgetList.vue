<template>
  <BContainer>
    <BRow>
      <h2>
        Widgets
      </h2>

      <div v-if="loading" class="loading">
        Loading widgets...
      </div>

      <div v-else-if="error" class="error">
        Error: {{ error }}
        <BButton variant="warning" @click="fetchWidgetsAsync">Retry</BButton>
      </div>
      <!-- Success/Error Messages -->
      <BAlert v-if="tableChangedMessage"
              variant="success"
              :model-value="true"
              class="mt-3"
              dismissible
              @dismissed="tableChangedMessage = ''">
        {{ tableChangedMessage }}
      </BAlert>
    </BRow>

    <BRow>

      <BCol>
        <WidgetCreateForm @widget-created="tableChangedAsync" />
      </BCol>
    </BRow>

    <BRow>
      <BCol>
        <!-- Table -->
        <div class="overflow-auto">

          <BTable id="widgets-table"
                  :items="widgets"
                  :per-page="perPage"
                  :current-page="currentPage"
                  :fields="fields"
                  :responsive="true"
                  striped
                  hover
                  small>

            <template #cell(price)="data">
              ${{data.value.toFixed(2)}}
            </template>

            <template #cell(category)="data">
              {{ formatCategory(data.value) }}
            </template>

            <template #cell(dateAddedUtc)="data">
              {{ formatDate(data.value) }}
            </template>

            <template #cell(actions)="data">
              <BButton variant="primary"
                       size="sm"
                       @click="viewWidget(data.item)"
                       :disabled="loading">
                View
              </BButton>

              <BButton variant="danger"
                       size="sm"
                       @click="deleteWidget(data.item)"
                       :disabled="loading">
                Delete
              </BButton>
            </template>

          </BTable>

        </div>
      </BCol>
    </BRow>
    <BRow>

      <!-- Per Page Column -->
      <BCol>
        <BPagination v-model="currentPage"
                     :total-rows="rows"
                     :per-page="perPage"
                     aria-controls="widgets-table" />
      </BCol>
      <BCol class="">
        <div class="d-flex justify-content-end">
          <BFormGroup label="Per page"
                      label-align-sm="right"
                      label-size="sm"
                      label-for="per-page-select"
                      class="w-25">

            <BFormSelect id="per-page-select"
                         v-model="perPage"
                         :options="pageOptions"
                         size="sm" />
          </BFormGroup>
        </div>
      </BCol>
    </BRow>
  </BContainer>

  <!-- Delete Confirmation Modal -->
  <BModal v-model="showDeleteModal"
          title="Confirm Delete"
          @ok="confirmDeleteAsync"
          @cancel="cancelWidgetDeletion"
          ok-variant="danger"
          ok-title="Delete"
          cancel-title="Cancel">
    <p>Are you sure you want to delete "<strong>{{ widgetToDelete?.name }}</strong>"?</p>
    <p class="text-muted">This action cannot be undone.</p>
  </BModal>

  <!-- Details Modal -->
  <WidgetDetail v-model="showDetailsModal"
                 :widget="selectedWidget"
                 :categories="categories"/>

</template>

<script setup lang="ts">

  import { ref, onMounted, computed } from 'vue';
  import WidgetDetail from './WidgetDetail.vue'
  import WidgetCreate from './WidgetCreateForm.vue';

  import { widgetService } from '../../services/WidgetService.js';
  import { TableFieldRaw } from 'bootstrap-vue-next';

  const perPage = ref(5);
  const currentPage = ref(1);

  const loading = ref(false);
  const error = ref(null);

  const widgets = ref([]);

  const tableChangedMessage = ref('');

  // Categories to format the displayed names.
  const categories = ref([]);

  // Details modal state
  const showDetailsModal = ref(false);
  const selectedWidget = ref(null);

  // Delete modal state
  const showDeleteModal = ref(false);
  const widgetToDelete = ref(null);

  const pageOptions = [
    { value: 5, text: '5' },
    { value: 10, text: '10' },
    { value: 15, text: '15' },
  ]

  const fields: TableFieldRaw<Widget>[] = [
    {
      key: "name",
      sortable: true,
    },
    {
      key: "price",
      sortable: true,
    },
    {
      key: "category",
      sortable: true,
    },
    {
      key: "dateAddedUtc",
      label: "Date Added",
      sortable: true,
    },
    {
      key: "actions",
      label: "Actions",
      sortable: false,
    },
  ];

  const rows = computed(() => widgets.value.length);
   
  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString('en-AU', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    })
  }

  const formatCategory = (categoryIndex) => {
    const category = categories.value.find((cat) => { return cat.value === categoryIndex });
    return category ? category.text : categoryIndex; 
  }

  const viewWidget = (widget) => {
    selectedWidget.value = widget;
    showDetailsModal.value = true;
  }

  const deleteWidget = (widget) => {
    widgetToDelete.value = widget;
    showDeleteModal.value = true;
  }

  const cancelWidgetDeletion = () => {
    widgetToDelete.value = null;
    showDeleteModal.value = false;
  }

  const confirmDeleteAsync = async () => {

    if (!widgetToDelete) {
      return;
    }

    try {
      await widgetService.deleteWidgetAsync(widgetToDelete.value.id);
      tableChangedMessage.value = `Widget "${widgetToDelete.value.name}" deleted successfully`;
      await fetchWidgetsAsync();
    } catch (err) {
      error.value = `Failed to delete widget: ${err.message}`;
    }
  }

  const tableChangedAsync = async (message) => {
    tableChangedMessage.value = message;
    await fetchWidgetsAsync();
  }

  // Fetches and maps the categories to enable formatting of the
  // items in the table. 
  const fetchCategoriesAsync = async () => {
    try {
      const data = await widgetService.getCategoriesAsync();
      categories.value = data;
    } catch (error) {
      console.error('Error fetching categories:', error);
    }
  }

  const fetchWidgetsAsync = async () => {

    loading.value = true;
    error.value = null;

    try {
      const data = await widgetService.getWidgetsAsync();
      widgets.value = data;
    } catch (err) {
      error.value = err.message;
    } finally {
      loading.value = false;
    }
  }

  onMounted(() => {
    try {
      Promise.all([fetchWidgetsAsync(), fetchCategoriesAsync()])
    } catch (error) {
      error.value = 'Failed to load data';
    }
  });

</script>
