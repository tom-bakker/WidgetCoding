import { apiService } from './apiService.js';

export const widgetService = {

  async getWidgetsAsync() {
    return apiService.getAsync('/widgets');
  },

  async getWidgetAsync(id) {
    return apiService.getAsync(`/widgets/${id}`);
  },

  async createWidgetAsync(widgetData) {
    return apiService.postAsync('/widgets', widgetData);
  },

  async updateWidgetAsync(id, widgetData) {
    return apiService.putAsync(`/widgets/${id}`, widgetData);
  },

  async deleteWidgetAsync(id) {
    return apiService.deleteAsync(`/widgets/${id}`);
  },

  async getCategoriesAsync() {
    return apiService.getAsync('/widgets/categories');
  }
};
