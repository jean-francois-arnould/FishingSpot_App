window.fishingSpotMap = {
    maps: {},

    render: function (elementId, spots) {
        const element = document.getElementById(elementId);
        if (!element || !window.L) {
            return;
        }

        this.dispose(elementId);

        const validSpots = (spots || [])
            .map((spot) => ({
                name: spot.locationName || spot.LocationName || 'Spot',
                latitude: Number(spot.latitude ?? spot.Latitude),
                longitude: Number(spot.longitude ?? spot.Longitude),
                count: Number(spot.count ?? spot.Count ?? 0),
                speciesCount: Number(spot.speciesCount ?? spot.SpeciesCount ?? 0),
                score: Number(spot.productivityScore ?? spot.ProductivityScore ?? 0),
                bestScore: Number(spot.bestSessionScore ?? spot.BestSessionScore ?? 0),
                averageLength: Number(spot.averageLength ?? spot.AverageLength ?? 0),
                species: spot.species || spot.Species || []
            }))
            .filter((spot) => Number.isFinite(spot.latitude) && Number.isFinite(spot.longitude));

        if (!validSpots.length) {
            element.innerHTML = '';
            return;
        }

        const map = L.map(elementId, {
            scrollWheelZoom: false,
            tap: true
        });

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '&copy; OpenStreetMap contributors'
        }).addTo(map);

        const bounds = [];
        validSpots.forEach((spot) => {
            const radius = Math.max(9, Math.min(28, 8 + spot.score / 5));
            const marker = L.circleMarker([spot.latitude, spot.longitude], {
                radius: radius,
                color: '#1f6f54',
                weight: 2,
                fillColor: spot.score >= 80 ? '#d99b22' : '#2f9b7c',
                fillOpacity: 0.72
            }).addTo(map);

            const popup = [
                `<strong>${this.escape(spot.name)}</strong>`,
                `${spot.count} prise${spot.count > 1 ? 's' : ''}`,
                `${spot.speciesCount} espece${spot.speciesCount > 1 ? 's' : ''}`,
                `Score spot ${spot.score.toFixed(0)}/100`,
                spot.bestScore ? `Meilleure session ${spot.bestScore.toFixed(0)}/100` : '',
                spot.averageLength ? `Taille moyenne ${spot.averageLength.toFixed(0)} cm` : '',
                spot.species.length ? `<small>${this.escape(spot.species.slice(0, 4).join(', '))}</small>` : ''
            ].filter(Boolean).join('<br>');

            marker.bindPopup(popup);
            bounds.push([spot.latitude, spot.longitude]);
        });

        if (bounds.length === 1) {
            map.setView(bounds[0], 13);
        } else {
            map.fitBounds(bounds, { padding: [24, 24] });
        }

        setTimeout(() => map.invalidateSize(), 150);
        this.maps[elementId] = map;
    },

    dispose: function (elementId) {
        const map = this.maps[elementId];
        if (map) {
            map.remove();
            delete this.maps[elementId];
        }
    },

    escape: function (value) {
        return String(value)
            .replaceAll('&', '&amp;')
            .replaceAll('<', '&lt;')
            .replaceAll('>', '&gt;')
            .replaceAll('"', '&quot;')
            .replaceAll("'", '&#039;');
    }
};
