// Adapter API thật, được sử dụng bởi live.html; các không gian demo vẫn độc lập.
export class ApiError extends Error {
  constructor(status, problem = {}) {
    super(problem.title || 'Không thể xử lý yêu cầu. Vui lòng thử lại.');
    this.status = status;
    this.code = problem.code || `HTTP_${status}`;
    this.fields = problem.field_errors || problem.errors || {};
    this.requestId = problem.request_id;
  }
}

export class HpuniApi {
  constructor({ base = '/api/v1', fetchImpl = globalThis.fetch, timeoutMs = 15000 } = {}) {
    this.base = base.replace(/\/$/, '');
    this.fetch = fetchImpl.bind(globalThis);
    this.timeoutMs = timeoutMs;
    this.csrf = null;
    this.csrfPending = null;
  }

  async refreshCsrf() {
    if (this.csrfPending) return this.csrfPending;
    this.csrfPending = (async () => {
      const response = await this.fetch(`${this.base}/auth/csrf`, {
        credentials: 'same-origin', cache: 'no-store', signal: AbortSignal.timeout(this.timeoutMs),
        headers: { Accept: 'application/json' }
      });
      if (!response.ok) throw new ApiError(response.status, await this.problem(response));
      const { token } = await response.json();
      if (!token) throw new Error('Máy chủ không cung cấp mã xác nhận biểu mẫu.');
      this.csrf = token;
      return token;
    })();
    try { return await this.csrfPending; }
    finally { this.csrfPending = null; }
  }

  async problem(response) {
    try { return await response.json(); } catch { return {}; }
  }

  async request(path, { method = 'GET', data, idempotencyKey } = {}) {
    const headers = { Accept: 'application/json' };
    if (!['GET', 'HEAD', 'OPTIONS'].includes(method)) {
      headers['X-CSRF-TOKEN'] = this.csrf || await this.refreshCsrf();
    }
    if (data !== undefined) headers['Content-Type'] = 'application/json';
    if (idempotencyKey) headers['Idempotency-Key'] = idempotencyKey;
    const response = await this.fetch(`${this.base}${path}`, {
      method, credentials: 'same-origin', cache: 'no-store', headers,
      signal: AbortSignal.timeout(this.timeoutMs), body: data === undefined ? undefined : JSON.stringify(data)
    });
    if (!response.ok) {
      const problem = await this.problem(response);
      if (response.status === 401 || problem.code === 'CSRF_INVALID') this.csrf = null;
      throw new ApiError(response.status, problem);
    }
    return response.status === 204 ? null : response.json();
  }

  async login(userName, password) {
    const me = await this.request('/auth/login', { method: 'POST', data: { userName, password } });
    this.csrf = null;
    await this.refreshCsrf();
    return me;
  }
  async logout() {
    await this.request('/auth/logout', { method: 'POST' });
    this.csrf = null;
    await this.refreshCsrf();
  }
  async changePassword(currentPassword, newPassword) {
    await this.request('/auth/change-password', { method: 'POST', data: { currentPassword, newPassword } });
    this.csrf = null;
    await this.refreshCsrf();
  }
  me() { return this.request('/auth/me'); }
  departments() { return this.request('/departments'); }
  rooms(filters = {}) { return this.request(`/rooms?${new URLSearchParams(filters)}`); }
  roomAvailability(id, date) { return this.request(`/rooms/${encodeURIComponent(id)}/availability?${new URLSearchParams({ date })}`); }
  roomRequests(filters = {}) { return this.request(`/room-requests?${new URLSearchParams(filters)}`); }
  roomRequest(id, review = false) { return this.request(`/room-requests/${encodeURIComponent(id)}?review=${review}`); }
  createRoomRequest(data, key) { return this.keyed('/room-requests', data, key); }
  roomAction(id, action, data, key) {
    if (!['submit', 'approve', 'reject', 'cancel'].includes(action)) throw new Error('Thao tác không hợp lệ.');
    return this.keyed(`/room-requests/${encodeURIComponent(id)}/${action}`, data, key);
  }
  saveRoomDraft(id, data) { return this.request(`/room-requests/${encodeURIComponent(id)}`, { method: 'PUT', data }); }
  saveRoomLog(id, data) { return this.request(`/room-requests/${encodeURIComponent(id)}/log`, { method: 'PUT', data }); }
  reviewRoomLog(id, data) { return this.request(`/room-requests/${encodeURIComponent(id)}/log/review`, { method: 'POST', data }); }
  keyed(path, data, key) {
    if (!key) throw new Error('Cần khóa gửi lặp; giữ nguyên khóa khi thử lại cùng thao tác.');
    return this.request(path, { method: 'POST', data, idempotencyKey: key });
  }
}
