import assert from 'node:assert/strict';
import { writeFile } from 'node:fs/promises';

const baseUrl = process.env.HPUNI_SMOKE_URL || 'http://127.0.0.1:5080';
const checks = [];
const cases = [
  ['GET', '/health/live', 200], ['GET', '/health/ready', 200],
  ['GET', '/openapi/v1.json', 200], ['GET', '/api/v1/auth/csrf', 200],
  ['GET', '/api/v1/auth/me', 401], ['GET', '/api/v1/audit-events', 401],
  ['POST', '/api/v1/auth/login', 400]
];
let schema;
for (const [method, path, expected] of cases) {
  const response = await fetch(baseUrl + path, {
    method, redirect: 'manual', signal: AbortSignal.timeout(15000),
    ...(method === 'POST' ? { headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ userName: 'nonexistent.test', password: 'NotRealAccount123' }) } : {})
  });
  checks.push({ method, path, expected, actual: response.status, passed: response.status === expected });
  assert.equal(response.status, expected, `${method} ${path}`);
  if (path === '/openapi/v1.json') schema = await response.json();
}
const report = { at: new Date().toISOString(), baseUrl, checks, openapiPathCount: Object.keys(schema.paths).length };
await writeFile(new URL('../docs/smoke-http.json', import.meta.url), JSON.stringify(report, null, 2) + '\n', 'utf8');
console.log(JSON.stringify(report, null, 2));
