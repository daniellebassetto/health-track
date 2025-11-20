// Mobile Enhancements JavaScript

(function() {
    'use strict';

    // Mobile detection
    const isMobile = window.innerWidth <= 768;
    const isTouch = 'ontouchstart' in window || navigator.maxTouchPoints > 0;

    // Initialize mobile enhancements when DOM is ready
    document.addEventListener('DOMContentLoaded', function() {
        initMobileEnhancements();
        initTouchEnhancements();
        initViewportAdjustments();
        initAccessibilityEnhancements();
    });

    function initMobileEnhancements() {
        // Add mobile class to body
        if (isMobile) {
            document.body.classList.add('mobile-device');
        }

        // Improve navbar collapse behavior
        const navbarToggler = document.querySelector('.navbar-toggler');
        const navbarCollapse = document.querySelector('.navbar-collapse');
        
        if (navbarToggler && navbarCollapse) {
            // Close navbar when clicking outside
            document.addEventListener('click', function(e) {
                if (!navbarCollapse.contains(e.target) && !navbarToggler.contains(e.target)) {
                    const bsCollapse = bootstrap.Collapse.getInstance(navbarCollapse);
                    if (bsCollapse && navbarCollapse.classList.contains('show')) {
                        bsCollapse.hide();
                    }
                }
            });

            // Close navbar when clicking on nav links (mobile)
            if (isMobile) {
                const navLinks = navbarCollapse.querySelectorAll('.nav-link');
                navLinks.forEach(link => {
                    link.addEventListener('click', function() {
                        const bsCollapse = bootstrap.Collapse.getInstance(navbarCollapse);
                        if (bsCollapse && navbarCollapse.classList.contains('show')) {
                            setTimeout(() => bsCollapse.hide(), 150);
                        }
                    });
                });
            }
        }

        // Improve form interactions on mobile
        const formControls = document.querySelectorAll('.form-control, .form-select');
        formControls.forEach(control => {
            // Add focus classes for better visual feedback
            control.addEventListener('focus', function() {
                this.closest('.form-group, .form-floating')?.classList.add('focused');
            });
            
            control.addEventListener('blur', function() {
                this.closest('.form-group, .form-floating')?.classList.remove('focused');
            });
        });

        // Improve table responsiveness
        const tables = document.querySelectorAll('.table');
        tables.forEach(table => {
            if (!table.closest('.table-responsive')) {
                const wrapper = document.createElement('div');
                wrapper.className = 'table-responsive';
                table.parentNode.insertBefore(wrapper, table);
                wrapper.appendChild(table);
            }
        });

        // Add loading states to buttons
        const buttons = document.querySelectorAll('button[type="submit"], .btn-primary');
        buttons.forEach(button => {
            button.addEventListener('click', function() {
                if (this.form && this.form.checkValidity()) {
                    this.classList.add('loading');
                    this.disabled = true;
                    
                    // Re-enable after 5 seconds as fallback
                    setTimeout(() => {
                        this.classList.remove('loading');
                        this.disabled = false;
                    }, 5000);
                }
            });
        });
    }

    function initTouchEnhancements() {
        if (!isTouch) return;

        // Add touch class to body
        document.body.classList.add('touch-device');

        // Improve touch feedback for interactive elements
        const interactiveElements = document.querySelectorAll('button, .btn, .nav-link, .card, .dropdown-item');
        
        interactiveElements.forEach(element => {
            element.addEventListener('touchstart', function() {
                this.classList.add('touch-active');
            });
            
            element.addEventListener('touchend', function() {
                setTimeout(() => {
                    this.classList.remove('touch-active');
                }, 150);
            });
            
            element.addEventListener('touchcancel', function() {
                this.classList.remove('touch-active');
            });
        });

        // Prevent double-tap zoom on buttons
        const buttons = document.querySelectorAll('button, .btn');
        buttons.forEach(button => {
            button.addEventListener('touchend', function(e) {
                e.preventDefault();
                this.click();
            });
        });

        // Improve scroll behavior
        document.body.style.webkitOverflowScrolling = 'touch';
    }

    function initViewportAdjustments() {
        // Handle viewport height changes (mobile browsers)
        function setViewportHeight() {
            const vh = window.innerHeight * 0.01;
            document.documentElement.style.setProperty('--vh', `${vh}px`);
        }

        setViewportHeight();
        window.addEventListener('resize', setViewportHeight);
        window.addEventListener('orientationchange', function() {
            setTimeout(setViewportHeight, 100);
        });

        // Prevent zoom on input focus (iOS)
        if (/iPad|iPhone|iPod/.test(navigator.userAgent)) {
            const inputs = document.querySelectorAll('input, select, textarea');
            inputs.forEach(input => {
                if (parseFloat(getComputedStyle(input).fontSize) < 16) {
                    input.style.fontSize = '16px';
                }
            });
        }

        // Handle safe area insets
        if (CSS.supports('padding: env(safe-area-inset-top)')) {
            document.documentElement.classList.add('has-safe-area');
        }
    }

    function initAccessibilityEnhancements() {
        // Improve focus management
        const focusableElements = document.querySelectorAll(
            'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
        );

        // Add skip link functionality
        const skipLink = document.createElement('a');
        skipLink.href = '#main-content';
        skipLink.textContent = 'Pular para o conteúdo principal';
        skipLink.className = 'skip-link sr-only';
        skipLink.style.cssText = `
            position: absolute;
            top: -40px;
            left: 6px;
            background: var(--health-primary);
            color: white;
            padding: 8px;
            text-decoration: none;
            border-radius: 4px;
            z-index: 9999;
            transition: top 0.3s;
        `;
        
        skipLink.addEventListener('focus', function() {
            this.style.top = '6px';
        });
        
        skipLink.addEventListener('blur', function() {
            this.style.top = '-40px';
        });
        
        document.body.insertBefore(skipLink, document.body.firstChild);

        // Add main content landmark if not exists
        const main = document.querySelector('main');
        if (main && !main.id) {
            main.id = 'main-content';
        }

        // Improve keyboard navigation
        document.addEventListener('keydown', function(e) {
            // Escape key closes modals and dropdowns
            if (e.key === 'Escape') {
                const openModal = document.querySelector('.modal.show');
                if (openModal) {
                    const bsModal = bootstrap.Modal.getInstance(openModal);
                    if (bsModal) bsModal.hide();
                }

                const openDropdown = document.querySelector('.dropdown-menu.show');
                if (openDropdown) {
                    const bsDropdown = bootstrap.Dropdown.getInstance(openDropdown.previousElementSibling);
                    if (bsDropdown) bsDropdown.hide();
                }

                const openCollapse = document.querySelector('.navbar-collapse.show');
                if (openCollapse) {
                    const bsCollapse = bootstrap.Collapse.getInstance(openCollapse);
                    if (bsCollapse) bsCollapse.hide();
                }
            }
        });
    }

    // Utility functions
    window.MobileUtils = {
        isMobile: () => isMobile,
        isTouch: () => isTouch,
        
        showToast: function(message, type = 'info') {
            const toast = document.createElement('div');
            toast.className = `alert alert-${type} mobile-toast`;
            toast.textContent = message;
            toast.style.cssText = `
                position: fixed;
                top: 20px;
                right: 20px;
                z-index: 9999;
                min-width: 250px;
                animation: slideInRight 0.3s ease;
            `;
            
            document.body.appendChild(toast);
            
            setTimeout(() => {
                toast.style.animation = 'slideOutRight 0.3s ease';
                setTimeout(() => toast.remove(), 300);
            }, 3000);
        },
        
        vibrate: function(pattern = [100]) {
            if ('vibrate' in navigator) {
                navigator.vibrate(pattern);
            }
        },
        
        scrollToTop: function() {
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        }
    };

    // Add CSS animations
    const style = document.createElement('style');
    style.textContent = `
        @keyframes slideInRight {
            from { transform: translateX(100%); opacity: 0; }
            to { transform: translateX(0); opacity: 1; }
        }
        
        @keyframes slideOutRight {
            from { transform: translateX(0); opacity: 1; }
            to { transform: translateX(100%); opacity: 0; }
        }
        
        .touch-active {
            opacity: 0.7;
            transform: scale(0.98);
        }
        
        .focused {
            box-shadow: 0 0 0 0.2rem rgba(40, 167, 69, 0.25);
        }
        
        .loading {
            position: relative;
            pointer-events: none;
        }
        
        .loading::after {
            content: '';
            position: absolute;
            top: 50%;
            right: 1rem;
            width: 16px;
            height: 16px;
            border: 2px solid transparent;
            border-top: 2px solid currentColor;
            border-radius: 50%;
            animation: spin 1s linear infinite;
        }
        
        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }
        
        /* Safe area support */
        .has-safe-area {
            padding-top: env(safe-area-inset-top);
            padding-bottom: env(safe-area-inset-bottom);
            padding-left: env(safe-area-inset-left);
            padding-right: env(safe-area-inset-right);
        }
        
        /* Viewport height fix */
        .full-height {
            height: 100vh;
            height: calc(var(--vh, 1vh) * 100);
        }
    `;
    document.head.appendChild(style);

})();