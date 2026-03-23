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
                console.warn('Web Share API with files not supported');
                return false;
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
                console.warn('Cannot share these files');
                return false;
            }

            await navigator.share(sharePayload);
            return true;
        } catch (error) {
            console.error('Error sharing file:', error);
            return false;
        }
    }
};
