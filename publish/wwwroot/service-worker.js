// FitUP Service Worker — PWA
// Cache-first para assets estáticos com versionamento automático
const CACHE_NAME = 'fitup-v2.0';
const STATIC_ASSETS = [
  '/',
  '/index.html',
  '/manifest.json',
  '/css/app.css',
  '/FitUP.styles.css',
  '/js/jspdf.umd.min.js',
  '/js/pdfExport.js',
  '/data/exercises.json',
  '/data/focus-mappings.json',
  '/data/workout-templates.json',
  // ícones PWA
  '/img/logof1-32.png',
  '/img/logof1-192.png',
  '/img/logof1-512.png',
  '/img/logof1-1024.png',
  // backgrounds
  '/img/Home.png',
  '/img/HomeL.png',
];

// ── Install: faz cache imediato dos assets estáticos críticos ──
self.addEventListener('install', (event) => {
  event.waitUntil(
    caches.open(CACHE_NAME).then((cache) => {
      return cache.addAll(STATIC_ASSETS);
    }).then(() => self.skipWaiting())
  );
});

// ── Activate: limpa caches antigos ──
self.addEventListener('activate', (event) => {
  event.waitUntil(
    caches.keys().then((keys) => {
      return Promise.all(
        keys.filter((key) => key !== CACHE_NAME).map((key) => caches.delete(key))
      );
    }).then(() => self.clients.claim())
  );
});

// ── Fetch: cache-first para estáticos, network-first para APIs ──
self.addEventListener('fetch', (event) => {
  const url = new URL(event.request.url);

  // Ignora chamadas para APIs externas (backend)
  if (url.pathname.startsWith('/api') || url.origin !== self.location.origin) {
    return;
  }

  // Cache-first para assets estáticos (framework, imagens, CSS, JS)
  if (
    url.pathname.startsWith('/_framework/') ||
    url.pathname.startsWith('/_content/') ||
    url.pathname.startsWith('/img/') ||
    url.pathname.startsWith('/img-dt/') ||
    url.pathname.startsWith('/img-gm/') ||
    url.pathname.startsWith('/css/') ||
    url.pathname.startsWith('/js/') ||
    url.pathname.startsWith('/data/') ||
    url.pathname === '/' ||
    url.pathname.endsWith('.html') ||
    url.pathname.endsWith('.json') ||
    url.pathname.endsWith('.webp') ||
    url.pathname.endsWith('.png') ||
    url.pathname.endsWith('.jpg') ||
    url.pathname.endsWith('.css') ||
    url.pathname.endsWith('.js') ||
    url.pathname.endsWith('.wasm') ||
    url.pathname.endsWith('.dll') ||
    url.pathname.endsWith('.dat')
  ) {
    event.respondWith(
      caches.match(event.request).then((cached) => {
        if (cached) {
          return cached;
        }
        return fetch(event.request).then((response) => {
          if (!response || response.status !== 200) return response;
          const clone = response.clone();
          caches.open(CACHE_NAME).then((cache) => cache.put(event.request, clone));
          return response;
        });
      })
    );
  }
  // Para qualquer outra requisição, vai direto para a rede
});