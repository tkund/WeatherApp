const form = document.getElementById('weather-form');
const cityInput = document.getElementById('city-input');
const weatherResult = document.getElementById('weather-result');
const status = document.getElementById('status');
const suggestionsList = document.getElementById('suggestions');

const cityName = document.getElementById('city-name');
const temperature = document.getElementById('temperature');
const condition = document.getElementById('condition');
const conditionIcon = document.getElementById('condition-icon');
const humidity = document.getElementById('humidity');
const wind = document.getElementById('wind');
const updated = document.getElementById('updated');

let debounceTimer;
let suggestions = [];
let activeIndex = -1;

cityInput.addEventListener('input', () => {
    clearTimeout(debounceTimer);
    const query = cityInput.value.trim();

    if (query.length < 2) {
        hideSuggestions();
        return;
    }

    debounceTimer = setTimeout(async () => {
        try {
            const response = await fetch(`/api/cities?query=${encodeURIComponent(query)}`);
            const items = await response.json();
            suggestions = Array.isArray(items) ? items : [];

            if (suggestions.length === 0) {
                hideSuggestions();
                return;
            }

            renderSuggestions();
        } catch {
            hideSuggestions();
        }
    }, 250);
});

cityInput.addEventListener('keydown', (event) => {
    if (suggestions.length === 0) {
        return;
    }

    if (event.key === 'ArrowDown') {
        event.preventDefault();
        activeIndex = Math.min(activeIndex + 1, suggestions.length - 1);
        highlightSuggestion();
    } else if (event.key === 'ArrowUp') {
        event.preventDefault();
        activeIndex = Math.max(activeIndex - 1, 0);
        highlightSuggestion();
    } else if (event.key === 'Enter' && activeIndex >= 0) {
        event.preventDefault();
        selectSuggestion(suggestions[activeIndex]);
    } else if (event.key === 'Escape') {
        hideSuggestions();
    }
});

form.addEventListener('submit', async (event) => {
    event.preventDefault();
    const city = cityInput.value.trim();
    await loadWeather(city);
});

function renderSuggestions() {
    suggestionsList.innerHTML = '';
    suggestions.forEach((item, index) => {
        const option = document.createElement('li');
        option.className = 'suggestion-item';
        option.textContent = `${item.city}, ${item.country}`;
        option.dataset.index = String(index);
        option.addEventListener('mousedown', (event) => {
            event.preventDefault();
            selectSuggestion(item);
        });
        suggestionsList.appendChild(option);
    });

    activeIndex = 0;
    suggestionsList.classList.remove('hidden');
    highlightSuggestion();
}

function highlightSuggestion() {
    const items = suggestionsList.querySelectorAll('.suggestion-item');
    items.forEach((item, index) => {
        item.classList.toggle('active', index === activeIndex);
    });
}

function selectSuggestion(item) {
    cityInput.value = `${item.city}, ${item.country}`;
    hideSuggestions();
    loadWeather(item.city);
}

function hideSuggestions() {
    suggestionsList.classList.add('hidden');
    suggestionsList.innerHTML = '';
    suggestions = [];
    activeIndex = -1;
}

async function loadWeather(city) {
    if (!city) {
        status.textContent = 'Please enter a city name.';
        return;
    }

    status.textContent = 'Fetching weather...';
    weatherResult.classList.add('hidden');

    try {
        const response = await fetch(`/api/weather?city=${encodeURIComponent(city)}`);
        const payload = await response.json();

        if (!response.ok) {
            throw new Error(payload.error || 'Unable to fetch weather data.');
        }

        cityName.textContent = payload.city;
        temperature.textContent = `${payload.temperatureC}°C`;
        condition.textContent = payload.condition;
        conditionIcon.textContent = getConditionIcon(payload.condition);
        humidity.textContent = `${payload.humidity}%`;
        wind.textContent = `${payload.windSpeedKph} km/h`;
        updated.textContent = `Updated ${new Date(payload.lastUpdatedUtc).toLocaleString()}`;

        weatherResult.classList.remove('hidden');
        status.textContent = '';
    } catch (error) {
        status.textContent = error.message;
    }
}

function getConditionIcon(conditionText) {
    const normalized = conditionText.toLowerCase();

    if (normalized.includes('clear')) {
        return '☀️';
    }

    if (normalized.includes('partly cloudy')) {
        return '⛅';
    }

    if (normalized.includes('cloud') || normalized.includes('overcast')) {
        return '☁️';
    }

    if (normalized.includes('fog')) {
        return '🌫️';
    }

    if (normalized.includes('rain') || normalized.includes('shower')) {
        return '🌧️';
    }

    if (normalized.includes('snow')) {
        return '❄️';
    }

    if (normalized.includes('thunder')) {
        return '⛈️';
    }

    return '🌤️';
}
