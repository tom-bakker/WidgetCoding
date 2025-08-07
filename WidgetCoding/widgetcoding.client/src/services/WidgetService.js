import { apiService } from './apiService.js';

const WIDGET_URI = '/widgets';

export const widgetService = {
  
  async getWidgetsAsync() {
    return apiService.getAsync(WIDGET_URI);
  },

  async getWidgetAsync(id) {
    return apiService.getAsync(`${WIDGET_URI}/${id}`);
  },

  async createWidgetAsync(widgetData) {
    return apiService.postAsync(WIDGET_URI, widgetData);
  },

  async updateWidgetAsync(id, widgetData) {
    return apiService.putAsync(`${WIDGET_URI}/${id}`, widgetData);
  },

  async deleteWidgetAsync(id) {
    return apiService.deleteAsync(`${WIDGET_URI}/${id}`);
  },

  async getCategoriesAsync() {
    return apiService.getAsync(`${WIDGET_URI}/categories`);
  }
};
