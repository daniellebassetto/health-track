const Masks = {
    cpf: function(value) {
        value = value.replace(/\D/g, '').substring(0, 11);
        value = value.replace(/(\d{3})(\d)/, '$1.$2');
        value = value.replace(/(\d{3})(\d)/, '$1.$2');
        value = value.replace(/(\d{3})(\d{1,2})$/, '$1-$2');
        return value;
    },
    
    rg: function(value) {
        value = value.replace(/\D/g, '').substring(0, 9);
        value = value.replace(/(\d{2})(\d)/, '$1.$2');
        value = value.replace(/(\d{3})(\d)/, '$1.$2');
        value = value.replace(/(\d{3})(\d{1})$/, '$1-$2');
        return value;
    },
    
    phone: function(value) {
        value = value.replace(/\D/g, '').substring(0, 11);
        value = value.replace(/(\d{2})(\d)/, '($1) $2');
        value = value.replace(/(\d{4,5})(\d{4})$/, '$1-$2');
        return value;
    },
    
    init: function() {
        const fields = [
            { id: 'Cpf', mask: this.cpf },
            { id: 'Rg', mask: this.rg },
            { id: 'Phone', mask: this.phone }
        ];
        
        fields.forEach(field => {
            const element = document.getElementById(field.id);
            if (element) {
                if (element.value) element.value = field.mask(element.value);
                
                element.addEventListener('input', function(e) {
                    e.target.value = field.mask(e.target.value);
                });
            }
        });
        
        document.querySelector('form').addEventListener('submit', function() {
            fields.forEach(field => {
                const element = document.getElementById(field.id);
                if (element && element.value) {
                    element.value = element.value.replace(/\D/g, '');
                }
            });
        });
    }
};