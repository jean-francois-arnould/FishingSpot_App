// Image Compression Helper pour Blazor
window.imageCompressionHelper = {
    compressImage: async function (base64Image, maxWidth, thumbnailSize, quality) {
        try {
            // Créer une image
            const img = await this.loadImage(base64Image);

            // Compression image principale
            const compressed = await this.resizeImage(img, maxWidth, quality);

            // Création thumbnail
            const thumbnail = await this.resizeImage(img, thumbnailSize, quality);

            return {
                base64Data: compressed,
                base64Thumbnail: thumbnail,
                originalSize: base64Image.length,
                compressedSize: compressed.length,
                thumbnailSize: thumbnail.length
            };
        } catch (error) {
            console.error('Image compression error:', error);
            return {
                base64Data: base64Image,
                base64Thumbnail: base64Image,
                originalSize: base64Image.length,
                compressedSize: base64Image.length,
                thumbnailSize: base64Image.length
            };
        }
    },

    loadImage: function (base64) {
        return new Promise((resolve, reject) => {
            const img = new Image();
            img.onload = () => resolve(img);
            img.onerror = reject;
            img.src = base64;
        });
    },

    resizeImage: function (img, maxWidth, quality) {
        const canvas = document.createElement('canvas');
        const ctx = canvas.getContext('2d');

        // Calculer les nouvelles dimensions
        let width = img.width;
        let height = img.height;

        if (width > maxWidth) {
            height = (height * maxWidth) / width;
            width = maxWidth;
        }

        // Redimensionner avec antialiasing
        canvas.width = width;
        canvas.height = height;

        // Améliorer la qualité du redimensionnement
        ctx.imageSmoothingEnabled = true;
        ctx.imageSmoothingQuality = 'high';

        ctx.drawImage(img, 0, 0, width, height);

        // Convertir en base64 JPEG
        return canvas.toDataURL('image/jpeg', quality / 100);
    }
};
