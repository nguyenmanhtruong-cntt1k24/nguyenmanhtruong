import { test } from 'node:test';
import assert from 'node:assert/strict';
import { HpuniApi, ApiError } from './api-client.mjs';

const json = (data, status = 200) => new Response(JSON.stringify(data), { status, headers: { 'Content-Type': 'application/json' } });

test('fetch chạy với ngữ cảnh global như trình duyệt yêu cầu', async () => {
  const api = new HpuniApi({ fetchImpl: async function () {
    assert.equal(this, globalThis);
    return json({ id: 'user' });
  } });
  assert.deepEqual(await api.me(), { id: 'user' });
});

test('login lấy token mới sau khi danh tính thay đổi, không lưu mật khẩu', async () => {
  const calls = [];
  let tokens = 0;
  const api = new HpuniApi({ fetchImpl: async (url, options) => {
    calls.push([url, options]);
    if (url.endsWith('/auth/csrf')) return json({ token: `token-${++tokens}` });
    return json({ id: 'user' });
  } });
  await api.login('test', 'TestOnly12345');
  assert.equal(tokens, 2);
  assert.equal(calls[1][1].headers['X-CSRF-TOKEN'], 'token-1');
  assert.equal(api.csrf, 'token-2');
  assert.ok(calls.every(([, options]) => options.credentials === 'same-origin' && options.cache === 'no-store'));
  assert.equal(api.password, undefined);
});

test('lỗi phiên bản cũ giữ mã lỗi và không tự gửi lại giao dịch', async () => {
  let commands = 0;
  const api = new HpuniApi({ fetchImpl: async url => {
    if (url.endsWith('/auth/csrf')) return json({ token: 'csrf' });
    commands++;
    return json({ code: 'STALE_VERSION', title: 'Tải lại', request_id: 'trace' }, 409);
  } });
  await assert.rejects(api.roomAction('id', 'submit', { version: 'old' }, 'same-key'), error =>
    error instanceof ApiError && error.status === 409 && error.code === 'STALE_VERSION' && error.requestId === 'trace');
  assert.equal(commands, 1);
});

test('khóa idempotency được giữ nguyên khi người gọi thử lại', async () => {
  const keys = [];
  const api = new HpuniApi({ fetchImpl: async (url, options) => {
    if (url.endsWith('/auth/csrf')) return json({ token: 'csrf' });
    keys.push(options.headers['Idempotency-Key']);
    return json({ id: 'one' });
  } });
  await api.createRoomRequest({}, 'unchanged-key');
  await api.createRoomRequest({}, 'unchanged-key');
  assert.deepEqual(keys, ['unchanged-key', 'unchanged-key']);
  assert.throws(() => api.createRoomRequest({}), /khóa gửi lặp/);
});

test('401 và lỗi CSRF buộc lấy lại token, lỗi HTML không làm hỏng xử lý', async () => {
  const api = new HpuniApi({ fetchImpl: async () => new Response('<html>Error</html>', { status: 401 }) });
  api.csrf = 'old';
  await assert.rejects(api.me(), error => error instanceof ApiError && error.status === 401);
  assert.equal(api.csrf, null);
});
