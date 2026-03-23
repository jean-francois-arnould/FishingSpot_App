// Analytics Helper (compatible Google Analytics 4)
window.analyticsHelper = {
    // Initialiser avec votre Measurement ID Google Analytics
    measurementId: 'G-XXXXXXXXXX', // À remplacer par votre ID

    trackPageView: function (pageName) {
        if (typeof gtag !== 'undefined') {
            gtag('event', 'page_view', {
                page_title: pageName,
                page_location: window.location.href,
                page_path: window.location.pathname
            });
        } else {
            console.log('Analytics: Page view -', pageName);
        }
    },

    trackEvent: function (eventName, properties) {
        if (typeof gtag !== 'undefined') {
            gtag('event', eventName, properties || {});
        } else {
            console.log('Analytics: Event -', eventName, properties);
        }
    },

    trackException: function (exceptionData) {
        if (typeof gtag !== 'undefined') {
            gtag('event', 'exception', {
                description: exceptionData.message,
                fatal: false,
                ...exceptionData.properties
            });
        } else {
            console.log('Analytics: Exception -', exceptionData);
        }
    },

    trackPerformance: function (metricName, value, properties) {
        if (typeof gtag !== 'undefined') {
            gtag('event', 'timing_complete', {
                name: metricName,
                value: Math.round(value),
                event_category: 'Performance',
                ...properties
            });
        } else {
            console.log('Analytics: Performance -', metricName, value);
        }

        // Web Vitals
        if ('PerformanceObserver' in window) {
            this.observeWebVitals();
        }
    },

    observeWebVitals: function () {
        // Observer Core Web Vitals (LCP, FID, CLS)
        if (this._webVitalsObserved) return;
        this._webVitalsObserved = true;

        try {
            // Largest Contentful Paint (LCP)
            new PerformanceObserver((list) => {
                for (const entry of list.getEntries()) {
                    if (typeof gtag !== 'undefined') {
                        gtag('event', 'web_vitals', {
                            name: 'LCP',
                            value: Math.round(entry.renderTime || entry.loadTime),
                            event_category: 'Web Vitals'
                        });
                    }
                }
            }).observe({ type: 'largest-contentful-paint', buffered: true });

            // First Input Delay (FID)
            new PerformanceObserver((list) => {
                for (const entry of list.getEntries()) {
                    if (typeof gtag !== 'undefined') {
                        gtag('event', 'web_vitals', {
                            name: 'FID',
                            value: Math.round(entry.processingStart - entry.startTime),
                            event_category: 'Web Vitals'
                        });
                    }
                }
            }).observe({ type: 'first-input', buffered: true });

            // Cumulative Layout Shift (CLS)
            let clsValue = 0;
            new PerformanceObserver((list) => {
                for (const entry of list.getEntries()) {
                    if (!entry.hadRecentInput) {
                        clsValue += entry.value;
                    }
                }
            }).observe({ type: 'layout-shift', buffered: true });

            // Envoyer CLS à la fin de la session
            window.addEventListener('visibilitychange', () => {
                if (document.visibilityState === 'hidden' && typeof gtag !== 'undefined') {
                    gtag('event', 'web_vitals', {
                        name: 'CLS',
                        value: Math.round(clsValue * 1000),
                        event_category: 'Web Vitals'
                    });
                }
            });
        } catch (error) {
            console.error('Error observing Web Vitals:', error);
        }
    }
};

// Initialiser Web Vitals observation
if (document.readyState === 'complete') {
    window.analyticsHelper.observeWebVitals();
} else {
    window.addEventListener('load', () => window.analyticsHelper.observeWebVitals());
}
