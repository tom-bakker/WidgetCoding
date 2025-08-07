<template>
  <BModal v-model="isVisible"
          :title="`${widget?.name || 'Unknown'}`"
          size="lg"
          @hidden="onModalHidden"
          ok-only
          ok-title="Close">
    <div v-if="widget" class="widget-details">
      <!-- Widget Information Card -->
      <BCard class="mb-3">
        <BCardHeader>
          <h5 class="mb-0">
            {{widget?.name || 'Unknown'}} Information
          </h5>
        </BCardHeader>
        <BCardBody>
          <BRow>
            <BCol md="6">
              <div class="detail-item mb-3">
                <label class="fw-bold text-muted">Name:</label>
                <p class="mb-0">{{ widget.name }}</p>
              </div>
            </BCol>
            <BCol md="6">
              <div class="detail-item mb-3">
                <label class="fw-bold text-muted">Price:</label>
                <p class="mb-0 fs-5 text-success">${{ widget.price.toFixed(2) }}</p>
              </div>
            </BCol>
          </BRow>
          <BRow>
            <BCol md="6">
              <div class="detail-item mb-3">
                <label class="fw-bold text-muted">Category:</label>
                <p class="mb-0">
                  <BBadge variant="primary">{{ formattedCategory }}</BBadge>
                </p>
              </div>
            </BCol>
            <BCol md="6">
              <div class="detail-item mb-3">
                <label class="fw-bold text-muted">Date Added:</label>
                <p class="mb-0">{{ formattedDate }}</p>
              </div>
            </BCol>
          </BRow>
        </BCardBody>
      </BCard>
    </div>

    <div v-else class="text-center py-4">
      <BSpinner variant="primary" />
      <p class="mt-2 text-muted">Loading widget details...</p>
    </div>

    <template #footer>
      <!-- Empty footer to disable dismiss buttons -->
      <div></div>
    </template>

  </BModal>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';

interface Widget {
  id: string;
  name: string;
  price: number;
  category: string;
  dateAddedUtc: string;
}

interface Category {
  value: string;
  text: string;
}

interface Props {
  modelValue: boolean;
  widget: Widget | null;
  categories?: Category[];
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void;
  (e: 'edit-widget', widget: Widget): void;
  (e: 'delete-widget', widget: Widget): void;
  (e: 'duplicate-widget', widget: Widget): void;
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: false,
  widget: null,
  categories: () => []
});

const emit = defineEmits<Emits>();

const isVisible = computed({
  get: () => props.modelValue,
  set: (value: boolean) => emit('update:modelValue', value)
});

const formattedDate = computed(() => {
  if (!props.widget?.dateAddedUtc) return 'N/A';
  return new Date(props.widget.dateAddedUtc).toLocaleDateString('en-AU', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  });
});

const formattedCategory = computed(() => {
  if (!props.widget?.category) return 'N/A';

  const category = props.categories.find((cat) => cat.value === props.widget?.category);
  return category ? category.text : props.widget.category;
});

const onModalHidden = () => {
  emit('update:modelValue', false);
};

</script>

<style scoped>
  .detail-item label {
    font-size: 0.875rem;
    margin-bottom: 0.25rem;
    display: block;
  }

  .widget-details .card {
    border: 1px solid #dee2e6;
    box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
  }

  .widget-details .card-header {
    background-color: #f8f9fa;
    border-bottom: 1px solid #dee2e6;
  }

    .widget-details .card-header h5 {
      color: #495057;
    }

  .gap-2 {
    gap: 0.5rem;
  }
</style>
