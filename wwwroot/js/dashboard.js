// Dashboard JavaScript enhancements
document.addEventListener('DOMContentLoaded', function() {
    
    // Animate stats cards on scroll
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver(function(entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, observerOptions);

    // Observe all stats cards
    document.querySelectorAll('.stats-card').forEach(card => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(30px)';
        card.style.transition = 'all 0.6s ease';
        observer.observe(card);
    });

    // Add click animation to buttons
    document.querySelectorAll('.btn').forEach(button => {
        button.addEventListener('click', function(e) {
            const ripple = document.createElement('span');
            const rect = this.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;
            
            ripple.style.width = ripple.style.height = size + 'px';
            ripple.style.left = x + 'px';
            ripple.style.top = y + 'px';
            ripple.classList.add('ripple');
            
            this.appendChild(ripple);
            
            setTimeout(() => {
                ripple.remove();
            }, 600);
        });
    });

    // Auto-dismiss alerts after 5 seconds
    setTimeout(() => {
        const alerts = document.querySelectorAll('.alert');
        alerts.forEach(alert => {
            if (alert.querySelector('.btn-close')) {
                alert.style.transition = 'all 0.5s ease';
                alert.style.opacity = '0';
                alert.style.transform = 'translateY(-20px)';
                setTimeout(() => {
                    alert.remove();
                }, 500);
            }
        });
    }, 5000);

    // Add loading state to buttons on form submission
    document.querySelectorAll('form').forEach(form => {
        form.addEventListener('submit', function() {
            const submitBtn = this.querySelector('button[type="submit"], input[type="submit"]');
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.innerHTML = '<i class="bi bi-hourglass-split me-1"></i> Processando...';
                submitBtn.classList.add('loading');
            }
        });
    });

    // Smooth scroll for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });

    // Add tooltip functionality
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Real-time clock update
    function updateClock() {
        const now = new Date();
        const timeString = now.toLocaleString('pt-BR', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
        
        const clockElement = document.querySelector('.last-access');
        if (clockElement) {
            clockElement.textContent = `Último acesso: ${timeString}`;
        }
    }

    // Update clock every minute
    setInterval(updateClock, 60000);

    // Add keyboard navigation
    document.addEventListener('keydown', function(e) {
        // Alt + D for Dashboard
        if (e.altKey && e.key === 'd') {
            e.preventDefault();
            const dashboardLink = document.querySelector('a[href*="Dashboard"]');
            if (dashboardLink) dashboardLink.click();
        }
        
        // Alt + E for Exams
        if (e.altKey && e.key === 'e') {
            e.preventDefault();
            const examsLink = document.querySelector('a[href*="Exams"]');
            if (examsLink) examsLink.click();
        }
        
        // Alt + P for Profile
        if (e.altKey && e.key === 'p') {
            e.preventDefault();
            const profileLink = document.querySelector('a[href*="Profile"]');
            if (profileLink) profileLink.click();
        }
    });
});

// CSS for ripple effect
const style = document.createElement('style');
style.textContent = `
    .btn {
        position: relative;
        overflow: hidden;
    }
    
    .ripple {
        position: absolute;
        border-radius: 50%;
        background: rgba(255, 255, 255, 0.6);
        transform: scale(0);
        animation: ripple-animation 0.6s linear;
        pointer-events: none;
    }
    
    @keyframes ripple-animation {
        to {
            transform: scale(4);
            opacity: 0;
        }
    }
`;
document.head.appendChild(style);