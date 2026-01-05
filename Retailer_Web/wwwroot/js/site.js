// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
(function () {
    const body = document.body;
    const toggler = document.querySelector('[data-sidebar-toggle]');
    if (!toggler) {
        return;
    }

    const targetSelector = toggler.getAttribute('data-target') || '#sidenav-main';
    const sidenav = document.querySelector(targetSelector);
    const SIDENAV_SHOW = 'g-sidenav-show';
    const SIDENAV_HIDDEN = 'g-sidenav-hidden';
    const SIDENAV_PINNED = 'g-sidenav-pinned';
    const DESKTOP_BREAKPOINT = 992; // Bootstrap lg breakpoint

    function isDesktop() {
        return window.innerWidth >= DESKTOP_BREAKPOINT;
    }

    function setPinnedState(forcePinned) {
        const shouldPin = forcePinned ?? isDesktop();
        body.classList.toggle(SIDENAV_PINNED, shouldPin);
    }

    function showSidebar(forcePinned) {
        body.classList.remove(SIDENAV_HIDDEN);
        body.classList.add(SIDENAV_SHOW);
        setPinnedState(forcePinned);
        if (sidenav) {
            sidenav.classList.add('show');
        }
    }

    function hideSidebar() {
        body.classList.remove(SIDENAV_SHOW, SIDENAV_PINNED);
        body.classList.add(SIDENAV_HIDDEN);
        if (sidenav) {
            sidenav.classList.remove('show');
        }
    }

    function toggleSidebar(event) {
        event.preventDefault();
        event.stopPropagation();
        event.stopImmediatePropagation();

        if (!sidenav) {
            return;
        }

        const isVisible = body.classList.contains(SIDENAV_SHOW) && !body.classList.contains(SIDENAV_HIDDEN);
        if (isVisible) {
            hideSidebar();
        } else {
            showSidebar();
        }
    }

    let lastViewportIsDesktop = isDesktop();

    function applyViewportDefaults(forceApply) {
        const desktopNow = isDesktop();
        if (!forceApply && desktopNow === lastViewportIsDesktop) {
            return;
        }

        lastViewportIsDesktop = desktopNow;
        if (desktopNow) {
            showSidebar(true);
        } else {
            hideSidebar();
        }
    }

    toggler.addEventListener('click', toggleSidebar);

    // Keep initial state consistent with layout expectations.
    if (!body.classList.contains(SIDENAV_SHOW) && !body.classList.contains(SIDENAV_HIDDEN)) {
        applyViewportDefaults(true);
    } else {
        setPinnedState();
    }

    let resizeTimeout;
    window.addEventListener('resize', () => {
        clearTimeout(resizeTimeout);
        resizeTimeout = setTimeout(() => {
            applyViewportDefaults(false);
        }, 150);
    });
})();

(function () {
    const triggers = document.querySelectorAll('[data-sidebar-section-trigger]');
    if (!triggers.length) {
        return;
    }

    const accordionHost = document.querySelector('[data-sidebar-accordion]');
    const useAccordion = accordionHost ? accordionHost.getAttribute('data-sidebar-accordion') !== 'false' : false;

    const sections = Array.from(triggers)
        .map(trigger => {
            const sectionKey = trigger.getAttribute('data-sidebar-section-trigger');
            if (!sectionKey) {
                return null;
            }

            const target = document.querySelector(`[data-sidebar-section="${sectionKey}"]`);
            if (!target) {
                return null;
            }

            return {
                trigger,
                target,
                parent: trigger.closest('.sidebar-section')
            };
        })
        .filter(Boolean);

    if (!sections.length) {
        return;
    }

    function setSectionState(section, open) {
        section.target.classList.toggle('show', open);
        section.trigger.classList.toggle('collapsed', !open);
        section.trigger.setAttribute('aria-expanded', String(open));
        if (section.parent) {
            section.parent.classList.toggle('menu-open', open);
        }
    }

    sections.forEach(section => {
        const defaultOpen = section.target.classList.contains('show');
        setSectionState(section, defaultOpen);

        section.trigger.addEventListener('click', event => {
            event.preventDefault();

            const isOpen = section.target.classList.contains('show');
            if (isOpen) {
                setSectionState(section, false);
                return;
            }

            if (useAccordion) {
                sections.forEach(other => {
                    if (other !== section) {
                        setSectionState(other, false);
                    }
                });
            }

            setSectionState(section, true);
        });
    });
})();

(function () {
    const loaderElement = document.getElementById('global-loader');
    if (!loaderElement) {
        window.__globalLoader = null;
        return;
    }

    let inflight = 0;
    const setVisibility = (show) => {
        loaderElement.classList.toggle('is-visible', show);
        loaderElement.setAttribute('aria-hidden', (!show).toString());
    };

    const controller = {
        begin: () => {
            inflight += 1;
            if (inflight === 1) {
                setVisibility(true);
            }
        },
        end: () => {
            inflight = Math.max(0, inflight - 1);
            if (inflight === 0) {
                setVisibility(false);
            }
        },
        reset: () => {
            inflight = 0;
            setVisibility(false);
        }
    };

    controller.reset();
    window.__globalLoader = controller;

    const originalFetch = window.fetch;
    if (originalFetch) {
        window.fetch = function (...args) {
            controller.begin();
            return originalFetch.apply(this, args)
                .then(response => {
                    controller.end();
                    return response;
                })
                .catch(error => {
                    controller.end();
                    throw error;
                });
        };
    }

    const originalSend = XMLHttpRequest.prototype.send;
    XMLHttpRequest.prototype.send = function (...args) {
        controller.begin();
        this.addEventListener('loadend', controller.end, { once: true });
        return originalSend.apply(this, args);
    };
})();

(function () {
    const controller = window.__globalLoader;
    if (!controller) {
        return;
    }

    const shouldIgnore = (element) => element && element.dataset && element.dataset.loader === 'ignore';

    document.addEventListener('submit', event => {
        if (event.defaultPrevented) {
            return;
        }

        const form = event.target;
        if (!(form instanceof HTMLFormElement)) {
            return;
        }

        if (shouldIgnore(form)) {
            return;
        }

        controller.begin();
    }, true);

    document.addEventListener('click', event => {
        if (event.defaultPrevented) {
            return;
        }

        const anchor = event.target.closest('a[href]');
        if (!anchor || shouldIgnore(anchor)) {
            return;
        }

        const href = anchor.getAttribute('href');
        if (!href || href.startsWith('#') || href.startsWith('javascript:')) {
            return;
        }

        if (anchor.target && anchor.target !== '_self') {
            return;
        }

        controller.begin();
    }, true);

    window.addEventListener('beforeunload', () => {
        controller.begin();
    });

    window.addEventListener('pageshow', () => {
        controller.reset();
    });
})();
