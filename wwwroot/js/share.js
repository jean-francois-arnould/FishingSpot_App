// Web Share API Helper
window.shareHelper = {
    canShare: function () {
        return navigator.share !== undefined;
    },

    share: async function (shareData) {
        try {
            if (!navigator.share) {
                console.warn('Web Share API not supported');
                return false;
            }

            await navigator.share(shareData);
            return true;
        } catch (error) {
            if (error.name === 'AbortError') {
                console.log('Share cancelled by user');
            } else {
                console.error('Error sharing:', error);
            }
            return false;
        }
    },

    shareFile: async function (shareData) {
        try {
            if (!navigator.share || !navigator.canShare) {
                console.warn('Web Share API with files not supported, téléchargement de l\'image...');
                // Fallback: télécharger l'image automatiquement
                this.downloadImage(shareData);
                return true; // Retourner true car le téléchargement a réussi
            }

            // Convertir base64 en File objects
            const files = await Promise.all(
                shareData.files.map(async (fileData) => {
                    const byteCharacters = atob(fileData.data);
                    const byteNumbers = new Array(byteCharacters.length);
                    for (let i = 0; i < byteCharacters.length; i++) {
                        byteNumbers[i] = byteCharacters.charCodeAt(i);
                    }
                    const byteArray = new Uint8Array(byteNumbers);
                    const blob = new Blob([byteArray], { type: fileData.type });
                    return new File([blob], fileData.name, { type: fileData.type });
                })
            );

            const sharePayload = {
                title: shareData.title,
                text: shareData.text,
                files: files
            };

            if (navigator.canShare && !navigator.canShare(sharePayload)) {
                console.warn('Cannot share these files, téléchargement de l\'image...');
                // Fallback: télécharger l'image
                this.downloadImage(shareData);
                return true;
            }

            await navigator.share(sharePayload);
            return true;
        } catch (error) {
            if (error.name === 'AbortError') {
                console.log('Share cancelled by user');
                return false;
            }
            console.error('Error sharing file, téléchargement de l\'image...', error);
            // Fallback: télécharger l'image en cas d'erreur
            this.downloadImage(shareData);
            return true;
        }
    },

    downloadImage: function (shareData) {
        try {
            if (!shareData.files || shareData.files.length === 0) {
                console.error('No files to download');
                return;
            }

            const fileData = shareData.files[0];
            const byteCharacters = atob(fileData.data);
            const byteNumbers = new Array(byteCharacters.length);
            for (let i = 0; i < byteCharacters.length; i++) {
                byteNumbers[i] = byteCharacters.charCodeAt(i);
            }
            const byteArray = new Uint8Array(byteNumbers);
            const blob = new Blob([byteArray], { type: fileData.type });

            // Créer un lien de téléchargement
            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = fileData.name;
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            URL.revokeObjectURL(url);

            console.log('✅ Image téléchargée:', fileData.name);
        } catch (error) {
            console.error('Error downloading image:', error);
        }
    }
};
