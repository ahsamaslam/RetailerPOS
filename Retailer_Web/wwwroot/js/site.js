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

    function showSidebar() {
        body.classList.remove(SIDENAV_HIDDEN);
        body.classList.add(SIDENAV_SHOW, SIDENAV_PINNED);
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

    toggler.addEventListener('click', toggleSidebar);

    // Keep initial state consistent with desktop layout expectations.
    if (!body.classList.contains(SIDENAV_SHOW) && !body.classList.contains(SIDENAV_HIDDEN)) {
        showSidebar();
    }
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
