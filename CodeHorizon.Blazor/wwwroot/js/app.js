window.codeHorizon = {
    setTheme: (theme) => {
        document.documentElement.setAttribute('data-bs-theme', theme);
        document.body.classList.toggle('theme-dark', theme === 'dark');
    },
    copyToClipboard: async (text) => {
        await navigator.clipboard.writeText(text);
    },
    share: async (title, text, url) => {
        if (navigator.share) {
            await navigator.share({ title, text, url });
        } else {
            await navigator.clipboard.writeText(url);
        }
    },
    getOnlineStatus: () => navigator.onLine,
    registerConnectivity: (dotNetRef) => {
        const handler = () => dotNetRef.invokeMethodAsync('SetOnlineStatus', navigator.onLine);
        window.addEventListener('online', handler);
        window.addEventListener('offline', handler);
    }
};
