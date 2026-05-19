// ===== NEPHROAI JAVASCRIPT =====

// Dark/Light Mode Toggle
function toggleTheme() {
    const html = document.documentElement;
    const isDark = html.getAttribute('data-theme') === 'dark';
    html.setAttribute('data-theme', isDark ? 'light' : 'dark');
    document.getElementById('themeIcon').className = isDark ? 'fas fa-moon' : 'fas fa-sun';
    localStorage.setItem('theme', isDark ? 'light' : 'dark');
}

// Apply saved theme on load
(function () {
    const saved = localStorage.getItem('theme');
    if (saved) {
        document.documentElement.setAttribute('data-theme', saved);
        const icon = document.getElementById('themeIcon');
        if (icon) icon.className = saved === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
    }
})();

// Sidebar toggle (mobile)
function toggleSidebar() {
    document.getElementById('sidebar').classList.toggle('open');
}

// Toast Notifications
function showToast(message, type = 'info') {
    const container = document.getElementById('toast-container');
    if (!container) return;
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.innerHTML = `<i class="fas fa-${type === 'success' ? 'check-circle' : type === 'error' ? 'times-circle' : 'info-circle'}"></i> ${message}`;
    toast.style.cssText = `
        position: fixed; bottom: 24px; right: 24px;
        background: var(--surface); color: var(--text);
        border-left: 4px solid ${type === 'success' ? '#10b981' : type === 'error' ? '#ef4444' : '#1a73e8'};
        padding: 14px 20px; border-radius: 10px; font-size: 14px;
        box-shadow: 0 8px 24px rgba(0,0,0,0.15);
        display: flex; align-items: center; gap: 10px;
        animation: slideIn 0.3s ease; z-index: 9999;
    `;
    container.appendChild(toast);
    setTimeout(() => toast.remove(), 3500);
}

// Add CSS for toast animation
const style = document.createElement('style');
style.textContent = `
    #toast-container { position: fixed; bottom: 24px; right: 24px; z-index: 9999; display: flex; flex-direction: column; gap: 10px; }
    @keyframes slideIn { from { transform: translateX(100%); opacity: 0; } to { transform: translateX(0); opacity: 1; } }
`;
document.head.appendChild(style);