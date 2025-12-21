(() => {
    const MIN_QUERY_LENGTH = 2;

    function initCompanyPicker(form) {
        const searchInput = form.querySelector('[data-company-search]');
        const results = form.querySelector('[data-company-results]');
        const idField = form.querySelector('[data-company-id]');
        const nameField = form.querySelector('[data-company-name]');
        const submitButton = form.querySelector('[data-company-submit]');
        const errorText = form.querySelector('[data-company-error]');

        if (!searchInput || !results || !idField || !nameField) {
            return;
        }

        const hideResults = () => results.classList.add('d-none');
        const showMessage = (message, cssClass = 'text-muted') => {
            results.classList.remove('d-none');
            results.innerHTML = `<div class="list-group-item ${cssClass}">${message}</div>`;
        };
        const clearSelection = () => {
            idField.value = '';
            nameField.value = '';
            if (submitButton) {
                submitButton.disabled = true;
            }
        };
        const setError = (message) => {
            if (errorText) {
                errorText.textContent = message ?? '';
            }
        };

        let debounceHandle;

        searchInput.addEventListener('input', () => {
            const query = searchInput.value.trim();
            clearSelection();
            setError('');

            if (query.length < MIN_QUERY_LENGTH) {
                hideResults();
                return;
            }

            clearTimeout(debounceHandle);
            debounceHandle = setTimeout(async () => {
                try {
                    showMessage('Searching…');

                    const response = await fetch(`/SuperAdmin/SearchCompanies?q=${encodeURIComponent(query)}`);

                    if (response.status === 401 || response.status === 403) {
                        window.location.href = `/Login?returnUrl=${encodeURIComponent(location.pathname + location.search)}`;
                        return;
                    }
                    else if (!response.ok) {
                        showMessage('Search failed. Please try again.', 'text-danger');
                        return;
                    }

                    const data = await response.json();
                    if (!Array.isArray(data) || data.length === 0) {
                        showMessage('No companies found.');
                        return;
                    }

                    results.innerHTML = '';
                    data.forEach(company => {
                        const button = document.createElement('button');
                        button.type = 'button';
                        button.className = 'list-group-item list-group-item-action';
                        button.textContent = company.name;

                        button.addEventListener('click', () => {
                            searchInput.value = company.name;
                            idField.value = company.id;
                            nameField.value = company.name;
                            if (submitButton) {
                                submitButton.disabled = false;
                            }
                            hideResults();
                        });

                        results.appendChild(button);
                    });

                    results.classList.remove('d-none');
                } catch {
                    showMessage('Unable to search right now.', 'text-danger');
                }
            }, 250);
        });

        document.addEventListener('click', (evt) => {
            if (!form.contains(evt.target)) {
                hideResults();
            }
        });

        form.addEventListener('submit', (evt) => {
            if (!idField.value) {
                evt.preventDefault();
                setError('Please pick a company from the list before continuing.');
                searchInput.focus();
            }
        });
    }

    document.addEventListener('DOMContentLoaded', () => {
        document
            .querySelectorAll('form[data-company-picker="true"]')
            .forEach(form => initCompanyPicker(form));
    });
})();