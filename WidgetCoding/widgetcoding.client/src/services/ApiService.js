class ApiService {
  constructor() {
    this.baseURL = 'https://localhost:44364/api'; 
    this.headers = {
      'Content-Type': 'application/json',
    };
  }

  async requestAsync(url, options = {}) {
    try {
      const response = await fetch(`${this.baseURL}${url}`, {
        headers: { ...this.headers, ...options.headers },
        ...options
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const contentType = response.headers.get('content-type');
      if (contentType && contentType.includes('application/json')) {
        return await response.json();
      }

      return response;
    } catch (error) {
      console.error('API request failed:', error);
      throw error;
    }
  }

  // GET request
  async getAsync(url) {
    return this.requestAsync(url, { method: 'GET' });
  }

  // POST request
  async postAsync(url, data) {
    return this.requestAsync(url, {
      method: 'POST',
      body: JSON.stringify(data)
    });
  }

  // PUT request
  async putAsync(url, data) {
    return this.requestAsync(url, {
      method: 'PUT',
      body: JSON.stringify(data)
    });
  }

  // DELETE request
  async deleteAsync(url) {
    return this.requestAsync(url, { method: 'DELETE' });
  }
}

// Create and export singleton instance
export const apiService = new ApiService();
