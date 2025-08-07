<template>
  <div>
    <BButton variant="primary" @click="isFormVisible = !isFormVisible">Add Widget</BButton>
    <BModal title="Add Widget" v-model="isFormVisible">
      <BForm @submit.prevent="submitWidgetAsync">
        <!-- Widget Name -->
        <BFormGroup id="name-group"
                    label="Widget Name:"
                    label-for="widget-name"
                    :invalid-feedback="nameFeedback"
                    :state="nameState"
                    description="Enter a name between 3 and 255 characters">
          <BFormInput id="widget-name"
                      v-model="form.name"
                      type="text"
                      placeholder="Enter widget name"
                      :state="nameState"
                      maxlength="255"
                      required />
        </BFormGroup>

        <!-- Widget Price -->
        <BFormGroup id="price-group"
                    label="Price ($):"
                    label-for="widget-price"
                    :invalid-feedback="priceFeedback"
                    :state="priceState"
                    description="Enter price with up to 2 decimal places (e.g., 19.99)">
          <BInputGroup>
            <BFormInput id="widget-price"
                        v-model="form.price"
                        type="number"
                        step="0.01"
                        min="0.01"
                        max="9999999999999999.99"
                        placeholder="0.00"
                        :state="priceState"
                        required />
          </BInputGroup>
        </BFormGroup>

        <!-- Category Selection -->
        <BFormGroup id="category-group"
                    label="Category:"
                    label-for="category-select"
                    :invalid-feedback="categoryFeedback"
                    :state="categoryState"
                    description="Select the widget category">
          <BFormSelect id="category-select"
                       v-model="form.category"
                       :options="categoryOptions"
                       :state="categoryState"
                       :disabled="isLoadingCategories">
            <template #first>
              <BFormSelectOption :value="null" disabled>
                {{ isLoadingCategories ? 'Loading categories...' : 'Select a category' }}
              </BFormSelectOption>
            </template>
          </BFormSelect>
        </BFormGroup>

        <!-- Action Buttons -->
        <div class="d-flex gap-2 justify-content-end">
          <BButton type="submit"
                   variant="primary"
                   :disabled="!isFormValid || isSubmitting">
            <BSpinner v-if="isSubmitting" small class="me-2" />
            {{ isSubmitting ? 'Creating...' : 'Create Widget' }}
          </BButton>
        </div>
      </BForm>

      <!-- Success/Error Messages -->
      <BAlert v-if="successMessage"
              variant="success"
              :model-value="true"
              class="mt-3"
              dismissible
              @dismissed="successMessage = ''">
        {{ successMessage }}
      </BAlert>

      <BAlert v-if="errorMessage"
              variant="danger"
              :model-value="true"
              class="mt-3"
              dismissible
              @dismissed="errorMessage = ''">
        {{ errorMessage }}
      </BAlert>

      <template #footer>
        <!-- Empty footer to disable ok and cancel button -->
        <div></div>
      </template>

    </BModal>
  </div>
  
</template>

<script setup>

  import { ref, defineEmits, computed, onMounted } from 'vue';
  import { widgetService } from '../../services/WidgetService.js';

  const MAX_NAME_LENGTH = 255;
  const MIN_NAME_LENGTH = 3;

  const MIN_PRICE = 0.00;
  const MAX_PRICE = 99999999999.99;

  const form = ref({
    name: '',
    price: '',
    category: null
  });
  const WIDGET_CREATED_EMIT_KEY = 'widget-created'; 
  const widgetCreatedEmit = defineEmits(['widget-created']);

  const editingWidget = ref(null);

  const categoryOptions = ref([]);
  const widgets = ref([]);

  const successMessage = ref('');
  const errorMessage = ref('');

  const isLoadingCategories = ref(false);
  const isSubmitting = ref(false);
  const isFormVisible = ref(false);

  const nameState = computed(() => {
    const name = form.value.name.trim();
    if (name.length === 0) {
      return null;
    }
    return name.length >= MIN_NAME_LENGTH && name.length <= MAX_NAME_LENGTH ? true : false;
  });

  const priceState = computed(() => {

    if (!form.value.price) {
      return null;
    }

    const price = parseFloat(form.value.price);

    if (isNaN(price)) {
      return false;
    }

    return (price >= MIN_PRICE && price <= MAX_PRICE);
  });

  const categoryState = computed(() => {
    return (form.value.category != null) ? true : null;
  });

  const nameFeedback = computed(() => {
    const name = form.value.name.trim();

    if (name.length === 0) {
      return '';
    }

    if (name.length < MIN_NAME_LENGTH) {
      return `Name must be at least "${MIN_NAME_LENGTH}" characters long`;
    }

    if (name.length > MAX_NAME_LENGTH) {
      return `Name cannot exceed "${MAX_NAME_LENGTH}" characters`;
    }
    return '';
  });

  const priceFeedback = computed(() => {
    if (!form.value.price) {
      return 'Price is required';
    }

    const price = parseFloat(form.value.price);

    if (isNaN(price)) {
      return 'Please enter a valid number';
    }

    if (price < MIN_PRICE) {
      return `Price must be at least "${MIN_PRICE}"`;
    }

    if (price > MAX_PRICE) {
      return 'Price is too large';
    }

    return '';
  });

  const categoryFeedback = computed(() => {
    return 'Please select a category';
  });

  const isFormValid = computed(() => {
   
    return nameState.value === true &&
      priceState.value === true &&
      form.value.category !== null;
  });

  const fetchCategoriesAsync = async () => {
    isLoadingCategories.value = true;
    try {
      const response = await widgetService.getCategoriesAsync();
      categoryOptions.value = response;
    } catch (error) {
      console.error('Error fetching categories:', error);
      errorMessage.value = 'Failed to load categories. Please refresh the page.';
    } finally {
      isLoadingCategories.value = false;
    }
  }

  const submitWidgetAsync = async () => {

    if (!isFormValid) {
      errorMessage.value = 'Please fill in all required fields correctly.';
      return;
    }

    isSubmitting.value = true;
    errorMessage.value = '';
    successMessage.value = '';

    const createdWidget = {
      name: form.value.name,
      price: parseFloat(form.value.price),
      category: form.value.category
    }

    try {
      const response = await widgetService.createWidgetAsync(createdWidget);

      successMessage.value = `Widget "${form.value.name}" created successfully!`;

    } catch (error) {
      console.error('Error saving widget:', error)
      errorMessage.value = error.message || 'Failed to save widget. Please try again.';
    } finally {

      widgetCreatedEmit('widget-created', successMessage.value);
      // Reset form
      resetForm();

      isFormVisible.value = false;
      isSubmitting.value = false;
    }
  }

  const refreshData = async() => {
    await Promise.all([
      fetchCategoriesAsync(),
    ])
  }

  const resetForm = () => {
    form.value = {
      name: '',
      price: '',
      category: null
    }
  }

  onMounted(() => {
    fetchCategoriesAsync();
  });

</script>
