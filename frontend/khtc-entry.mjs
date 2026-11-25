if(new URLSearchParams(location.search).get('mode')==='demo') {
 await import('./accounting.js?v=khtc-separate-20261125');
} else {
 await import('./server-finance.mjs?v=khtc-live-20261125');
 await import('./account-entry.mjs');
}
