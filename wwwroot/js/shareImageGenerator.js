// Générateur d'images de partage pour les prises de pêche
window.shareImageGenerator = {
    /**
     * Génère une belle image de partage avec la photo et les infos de la prise
     * @param {Object} catchData - Les données de la prise
     * @returns {Promise<Blob>} - L'image générée en Blob
     */
    generateShareImage: async function (catchData) {
        try {
            console.log('🎨 Génération de l\'image de partage...', catchData);

            // Configuration du canvas
            const canvas = document.createElement('canvas');
            const ctx = canvas.getContext('2d');

            // Dimensions de l'image finale (Instagram optimal: 1080x1350)
            canvas.width = 1080;
            canvas.height = 1350;

            // Couleurs et style
            const colors = {
                primary: '#0066cc',
                secondary: '#00b4d8',
                background: '#ffffff',
                text: '#023047',
                textLight: '#6c757d',
                accent: '#06d6a0'
            };

            // 1. Dessiner le fond blanc
            ctx.fillStyle = colors.background;
            ctx.fillRect(0, 0, canvas.width, canvas.height);

            // 2. Dessiner l'en-tête avec dégradé
            const headerHeight = 120;
            const gradient = ctx.createLinearGradient(0, 0, canvas.width, headerHeight);
            gradient.addColorStop(0, colors.primary);
            gradient.addColorStop(1, colors.secondary);
            ctx.fillStyle = gradient;
            ctx.fillRect(0, 0, canvas.width, headerHeight);

            // Logo/Titre dans l'en-tête
            ctx.fillStyle = '#ffffff';
            ctx.font = 'bold 48px Arial, sans-serif';
            ctx.textAlign = 'center';
            ctx.fillText('🎣 FishingSpot', canvas.width / 2, 75);

            // 3. Charger et dessiner la photo du poisson (si disponible)
            let photoYOffset = headerHeight + 40;

            if (catchData.photoUrl && !catchData.photoUrl.startsWith('offline_')) {
                try {
                    console.log('📸 Chargement de la photo:', catchData.photoUrl);
                    const img = await this.loadImage(catchData.photoUrl);
                    console.log('✅ Photo chargée:', img.width, 'x', img.height);

                    // Calculer les dimensions pour centrer et adapter la photo
                    const maxPhotoWidth = canvas.width - 80; // Marges de 40px
                    const maxPhotoHeight = 500;

                    const scale = Math.min(
                        maxPhotoWidth / img.width,
                        maxPhotoHeight / img.height,
                        1 // Ne pas agrandir si l'image est plus petite
                    );

                    const photoWidth = img.width * scale;
                    const photoHeight = img.height * scale;
                    const photoX = (canvas.width - photoWidth) / 2;

                    // Dessiner un cadre arrondi pour la photo
                    const borderRadius = 20;
                    this.drawRoundedRect(ctx, photoX - 10, photoYOffset - 10, photoWidth + 20, photoHeight + 20, borderRadius);
                    ctx.fillStyle = '#ffffff';
                    ctx.shadowColor = 'rgba(0, 0, 0, 0.15)';
                    ctx.shadowBlur = 20;
                    ctx.shadowOffsetY = 5;
                    ctx.fill();
                    ctx.shadowColor = 'transparent';

                    // Clip pour les coins arrondis
                    ctx.save();
                    this.drawRoundedRect(ctx, photoX, photoYOffset, photoWidth, photoHeight, borderRadius);
                    ctx.clip();
                    ctx.drawImage(img, photoX, photoYOffset, photoWidth, photoHeight);
                    ctx.restore();

                    photoYOffset += photoHeight + 50;
                    console.log('✅ Photo dessinée dans le canvas');
                } catch (photoError) {
                    console.warn('⚠️ Impossible de charger la photo, continuation sans photo:', photoError);
                    // Continuer sans la photo
                }
            } else {
                console.log('ℹ️ Pas de photo à charger ou photo offline');
            }

            // 4. Dessiner le nom du poisson (titre principal)
            ctx.fillStyle = colors.text;
            ctx.font = 'bold 56px Arial, sans-serif';
            ctx.textAlign = 'center';
            ctx.fillText(catchData.fishName, canvas.width / 2, photoYOffset);
            photoYOffset += 60;

            // 5. Dessiner les informations principales dans des cartes
            const cardY = photoYOffset;
            const cardWidth = 480;
            const cardHeight = 90;
            const cardSpacing = 20;
            const leftCardX = 60;
            const rightCardX = canvas.width - 60 - cardWidth;

            // Fonction pour dessiner une carte d'info
            const drawInfoCard = (x, y, icon, label, value) => {
                // Fond de la carte avec ombre
                ctx.shadowColor = 'rgba(0, 0, 0, 0.1)';
                ctx.shadowBlur = 15;
                ctx.shadowOffsetY = 3;
                this.drawRoundedRect(ctx, x, y, cardWidth, cardHeight, 15);
                ctx.fillStyle = '#f8f9fa';
                ctx.fill();
                ctx.shadowColor = 'transparent';

                // Icône
                ctx.font = '40px Arial, sans-serif';
                ctx.textAlign = 'left';
                ctx.fillText(icon, x + 20, y + 55);

                // Label
                ctx.fillStyle = colors.textLight;
                ctx.font = 'bold 18px Arial, sans-serif';
                ctx.fillText(label, x + 80, y + 32);

                // Valeur
                ctx.fillStyle = colors.text;
                ctx.font = 'bold 28px Arial, sans-serif';
                ctx.fillText(value, x + 80, y + 65);
            };

            // Carte Longueur
            drawInfoCard(leftCardX, cardY, '📏', 'LONGUEUR', `${catchData.length} cm`);

            // Carte Poids
            drawInfoCard(rightCardX, cardY, '⚖️', 'POIDS', `${catchData.weight} kg`);

            // Carte Lieu (si disponible)
            if (catchData.locationName) {
                drawInfoCard(leftCardX, cardY + cardHeight + cardSpacing, '📍', 'LIEU', 
                    this.truncateText(ctx, catchData.locationName, cardWidth - 100, 'bold 28px Arial, sans-serif'));
            }

            // Carte Date
            const dateStr = new Date(catchData.catchDate).toLocaleDateString('fr-FR', { 
                day: 'numeric', 
                month: 'short', 
                year: 'numeric' 
            });
            drawInfoCard(
                catchData.locationName ? rightCardX : leftCardX, 
                cardY + cardHeight + cardSpacing, 
                '📅', 
                'DATE', 
                dateStr
            );

            // 6. Dessiner les infos météo (si disponibles)
            let weatherY = cardY + (cardHeight * 2) + (cardSpacing * 2) + 30;

            if (catchData.weatherTemperature || catchData.weatherCondition) {
                ctx.fillStyle = colors.textLight;
                ctx.font = 'bold 22px Arial, sans-serif';
                ctx.textAlign = 'center';
                ctx.fillText('CONDITIONS MÉTÉO', canvas.width / 2, weatherY);
                weatherY += 40;

                let weatherText = '';
                if (catchData.weatherTemperature) {
                    weatherText = `${catchData.weatherTemperature}°C`;
                }
                if (catchData.weatherCondition) {
                    weatherText += ` • ${catchData.weatherCondition}`;
                }
                if (catchData.windSpeed) {
                    weatherText += ` • Vent: ${catchData.windSpeed} km/h`;
                }

                ctx.fillStyle = colors.text;
                ctx.font = '24px Arial, sans-serif';
                ctx.fillText(weatherText, canvas.width / 2, weatherY);
            }

            // 7. Dessiner le pied de page
            const footerY = canvas.height - 50;
            ctx.fillStyle = colors.textLight;
            ctx.font = '20px Arial, sans-serif';
            ctx.textAlign = 'center';
            ctx.fillText('Partagé via FishingSpot App', canvas.width / 2, footerY);

            // 8. Convertir en Blob puis en Base64 directement
            const blob = await new Promise(resolve => canvas.toBlob(resolve, 'image/jpeg', 0.95));

            if (!blob) {
                console.error('❌ Impossible de créer le blob');
                throw new Error('Canvas toBlob failed');
            }

            console.log('✅ Blob créé, conversion en base64...');

            // Convertir directement en base64
            const base64 = await this.blobToBase64(blob);

            console.log(`✅ Image générée avec succès (base64 length: ${base64.length})`);
            return base64;

        } catch (error) {
            console.error('❌ Erreur lors de la génération de l\'image:', error);
            throw error;
        }
    },

    /**
     * Charge une image depuis une URL
     */
    loadImage: function (url) {
        return new Promise((resolve, reject) => {
            const img = new Image();

            // Première tentative avec crossOrigin
            img.crossOrigin = 'anonymous';

            img.onload = () => {
                console.log('✅ Image chargée avec crossOrigin');
                resolve(img);
            };

            img.onerror = (error) => {
                console.warn('⚠️ Échec avec crossOrigin, tentative sans...', error);

                // Deuxième tentative SANS crossOrigin
                const img2 = new Image();

                img2.onload = () => {
                    console.log('✅ Image chargée sans crossOrigin');
                    resolve(img2);
                };

                img2.onerror = (error2) => {
                    console.error('❌ Impossible de charger l\'image:', error2);
                    reject(error2);
                };

                img2.src = url;
            };

            img.src = url;
        });
    },

    /**
     * Dessine un rectangle avec des coins arrondis
     */
    drawRoundedRect: function (ctx, x, y, width, height, radius) {
        ctx.beginPath();
        ctx.moveTo(x + radius, y);
        ctx.lineTo(x + width - radius, y);
        ctx.quadraticCurveTo(x + width, y, x + width, y + radius);
        ctx.lineTo(x + width, y + height - radius);
        ctx.quadraticCurveTo(x + width, y + height, x + width - radius, y + height);
        ctx.lineTo(x + radius, y + height);
        ctx.quadraticCurveTo(x, y + height, x, y + height - radius);
        ctx.lineTo(x, y + radius);
        ctx.quadraticCurveTo(x, y, x + radius, y);
        ctx.closePath();
    },

    /**
     * Tronque un texte pour qu'il rentre dans une largeur donnée
     */
    truncateText: function (ctx, text, maxWidth, font) {
        ctx.font = font;
        if (ctx.measureText(text).width <= maxWidth) {
            return text;
        }

        let truncated = text;
        while (ctx.measureText(truncated + '...').width > maxWidth && truncated.length > 0) {
            truncated = truncated.slice(0, -1);
        }
        return truncated + '...';
    },

    /**
     * Convertit un Blob en base64
     */
    blobToBase64: async function (blob) {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.onloadend = () => resolve(reader.result.split(',')[1]);
            reader.onerror = reject;
            reader.readAsDataURL(blob);
        });
    }
};
