import test from 'node:test';
import assert from 'node:assert/strict';
import {futureIntervals,freeIntervals,fitsInterval} from './availability.mjs';
test('Vietnam today clips suggestions past current time with entry buffer',()=>{
 const now=new Date('2026-11-10T05:43:25Z');
 const free=futureIntervals(freeIntervals([]),'2026-11-10',now);
 assert.deepEqual(free,[{start:'12:50',end:'18:00'}]);
 assert.equal(fitsInterval(free,'07:00','08:50'),false);
 assert.equal(fitsInterval(free,'12:50','13:40'),true);
});
test('past days and after closing have no suggestions; future days keep opening time',()=>{
 const now=new Date('2026-11-10T11:00:00Z');
 assert.deepEqual(futureIntervals(freeIntervals([]),'2026-11-10',now),[]);
 assert.deepEqual(futureIntervals(freeIntervals([]),'2026-11-09',now),[]);
 assert.deepEqual(futureIntervals(freeIntervals([]),'2026-11-11',now),[{start:'07:00',end:'18:00'}]);
});
test('future clipping preserves reservations and uses Vietnam date near midnight',()=>{
 const free=freeIntervals([{start:'13:00',end:'14:00',status:'approved'}]);
 assert.deepEqual(futureIntervals(free,'2026-11-10',new Date('2026-11-10T05:58:00Z')),[{start:'14:00',end:'18:00'}]);
 assert.deepEqual(futureIntervals(freeIntervals([]),'2026-11-10',new Date('2026-11-10T18:00:00Z')),[]);
});
