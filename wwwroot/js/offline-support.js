// ==========================================
// NETWORK STATUS DETECTION
// ==========================================
window.networkStatus = {
    dotNetRef: null,

    initialize: function(dotNetReference) {
        this.dotNetRef = dotNetReference;

        // Add event listeners for online/offline
        window.addEventListener('online', this.handleOnline.bind(this));
        window.addEventListener('offline', this.handleOffline.bind(this));

        // Return current status
        return navigator.onLine;
    },

    handleOnline: function() {
        if (this.dotNetRef) {
            this.dotNetRef.invokeMethodAsync('UpdateNetworkStatus', true);
        }
    },

    handleOffline: function() {
        if (this.dotNetRef) {
            this.dotNetRef.invokeMethodAsync('UpdateNetworkStatus', false);
        }
    },

    dispose: function() {
        window.removeEventListener('online', this.handleOnline.bind(this));
        window.removeEventListener('offline', this.handleOffline.bind(this));
        this.dotNetRef = null;
    }
};

// ==========================================
// INDEXED DB MANAGEMENT
// ==========================================
window.indexedDb = {
    dbName: 'FishingSpotDB',
    dbVersion: 1,
    db: null,

    // Store names for different data types
    stores: {
        catches: 'catches',
        species: 'species',
        setups: 'setups',
        rods: 'rods',
        reels: 'reels',
        lines: 'lines',
        lures: 'lures',
        leaders: 'leaders',
        hooks: 'hooks',
        brands: 'brands',
        syncQueue: 'syncQueue',
        userProfile: 'userProfile'
    },

    initialize: function() {
        return new Promise((resolve, reject) => {
            if (this.db) {
                resolve();
                return;
            }

            const request = indexedDB.open(this.dbName, this.dbVersion);

            request.onerror = () => {
                console.error('❌ Error opening IndexedDB:', request.error);
                reject(request.error);
            };

            request.onsuccess = () => {
                this.db = request.result;
                console.log('✅ IndexedDB opened successfully');
                resolve();
            };

            request.onupgradeneeded = (event) => {
                const db = event.target.result;
                console.log('🔧 Upgrading IndexedDB schema...');

                // Create object stores for each data type
                Object.values(this.stores).forEach(storeName => {
                    if (!db.objectStoreNames.contains(storeName)) {
                        db.createObjectStore(storeName, { keyPath: 'key' });
                        console.log(`✅ Created object store: ${storeName}`);
                    }
                });
            };
        });
    },

    setItem: function(storeName, key, jsonValue) {
        return new Promise((resolve, reject) => {
            if (!this.db) {
                reject(new Error('Database not initialized'));
                return;
            }

            const transaction = this.db.transaction([storeName], 'readwrite');
            const store = transaction.objectStore(storeName);
            const request = store.put({ key: key, value: jsonValue, timestamp: Date.now() });

            request.onsuccess = () => resolve();
            request.onerror = () => reject(request.error);
        });
    },

    getItem: function(storeName, key) {
        return new Promise((resolve, reject) => {
            if (!this.db) {
                reject(new Error('Database not initialized'));
                return;
            }

            const transaction = this.db.transaction([storeName], 'readonly');
            const store = transaction.objectStore(storeName);
            const request = store.get(key);

            request.onsuccess = () => {
                if (request.result) {
                    resolve(request.result.value);
                } else {
                    resolve(null);
                }
            };
            request.onerror = () => reject(request.error);
        });
    },

    getAllItems: function(storeName) {
        return new Promise((resolve, reject) => {
            if (!this.db) {
                reject(new Error('Database not initialized'));
                return;
            }

            const transaction = this.db.transaction([storeName], 'readonly');
            const store = transaction.objectStore(storeName);
            const request = store.getAll();

            request.onsuccess = () => {
                const results = request.result.map(item => item.value);
                resolve(results);
            };
            request.onerror = () => reject(request.error);
        });
    },

    deleteItem: function(storeName, key) {
        return new Promise((resolve, reject) => {
            if (!this.db) {
                reject(new Error('Database not initialized'));
                return;
            }

            const transaction = this.db.transaction([storeName], 'readwrite');
            const store = transaction.objectStore(storeName);
            const request = store.delete(key);

            request.onsuccess = () => resolve();
            request.onerror = () => reject(request.error);
        });
    },

    clearStore: function(storeName) {
        return new Promise((resolve, reject) => {
            if (!this.db) {
                reject(new Error('Database not initialized'));
                return;
            }

            const transaction = this.db.transaction([storeName], 'readwrite');
            const store = transaction.objectStore(storeName);
            const request = store.clear();

            request.onsuccess = () => resolve();
            request.onerror = () => reject(request.error);
        });
    },

    exists: function(storeName, key) {
        return new Promise((resolve, reject) => {
            if (!this.db) {
                reject(new Error('Database not initialized'));
                return;
            }

            const transaction = this.db.transaction([storeName], 'readonly');
            const store = transaction.objectStore(storeName);
            const request = store.get(key);

            request.onsuccess = () => resolve(request.result !== undefined);
            request.onerror = () => reject(request.error);
        });
    }
};
