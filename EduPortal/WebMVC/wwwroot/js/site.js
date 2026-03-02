function addNewSkill() {
    const newSkillInput = $('#new-skill-name');
    const skillName = newSkillInput.val().trim();
    if (!skillName) {
        newSkillInput.addClass('is-invalid');
        return;
    }
    const exist = skills.some(s => s.SkillName.toLowerCase() === skillName.toLowerCase());
    if (exist) { alert("This skill is already added!"); return; }
    newSkillInput.removeClass('is-invalid');
    skills.push({ SkillId: 0, SkillName: skillName });
    refreshSkillList();
    document.getElementById('new-skill-name').value = '';
}

function addExistingSkill(skillId, skillName) {
    const exist = skills.some(s => s.SkillId === skillId);
    if (exist) { alert("This skill is already added!"); return; }
    skills.push({ SkillId: skillId, SkillName: skillName });
    refreshSkillList();
}

function refreshSkillList() {
    const container = document.getElementById('skills-container');
    container.innerHTML = '';

    skills.forEach((s, i) => {
        const skillNameAttribute = `Skills[${i}].SkillName`;

        container.insertAdjacentHTML('beforeend', `
                <div class="skill-item d-flex align-items-center p-2 border rounded mb-2">
                    <input type="hidden" name="Skills[${i}].SkillId" value="${s.SkillId}" />
					<input type="hidden" name="${skillNameAttribute}" value="${s.SkillName}" />
                    <span class="flex-grow-1 me-2">${s.SkillName}</span>
                    <button type="button" class="btn btn-sm btn-outline-danger" onclick="removeSkill(${i})">Delete</button>
                </div>
                `);
    });
}

function removeSkill(index) {
    skills.splice(index, 1);
    refreshSkillList();
}

function toggleMaterialFields(type) {
    document.querySelectorAll('#new-material-form .material-type-fields').forEach(div => {
        div.style.display = 'none';
        div.querySelectorAll('input, select, textarea').forEach(el => el.disabled = true);
    });
    const active = document.querySelector(`#new-material-form .material-type-fields[data-type="${type}"]`);
    if (active) {
        active.style.display = 'block';
        active.querySelectorAll('input, select, textarea').forEach(el => el.disabled = false);
    }
}

function refreshMaterialList() {
    const container = document.getElementById('materials-container');
    const materialTypeNames = ["Video", "Article", "Book"];
    container.innerHTML = '';
    materials.forEach((m, i) => {
        const typeName = materialTypeNames[m.MaterialType];
        container.insertAdjacentHTML('beforeend', `
                    <div class="material-list-item d-flex justify-content-between align-items-center p-2 border rounded mb-2" data-index="${i}">
                        <div class="d-flex align-items-center">
                            <span class="drag-handle me-2" style="cursor: grab;">☰</span>
                            <span>${m.MaterialName} (${typeName})</span>
                        </div>
                        <div>
                            <button type="button" class="btn btn-sm btn-outline-danger" onclick="deleteMaterial(${i})">Delete</button>
                        </div>
                    </div>
                `);
    });
}

function saveNewMaterial() {
    clearMaterialErrors();

    const newMaterial = { MaterialId: 0, MaterialOrder: materials.length };
    const materialNameInput = document.getElementById('MaterialName');
    newMaterial.MaterialName = materialNameInput?.value?.trim() || '';
    const materialTypeSelect = document.getElementById('material-type-selector');
    newMaterial.MaterialType = parseInt(materialTypeSelect?.value || '0', 10);

    document.querySelectorAll('#new-material-form .material-type-fields input, #new-material-form .material-type-fields select').forEach(el => {
        if (el.offsetParent === null) return;
        const propName = el.name;
        if (!propName) return;
        let value;
        if (el.type === 'number') value = el.value ? parseInt(el.value, 10) : null;
        else if (el.type === 'file') value = el.files[0] || null;
        else value = el.value?.trim() || '';
        newMaterial[propName] = value;
    });

    if (newMaterial.MaterialType === 0) {
        const minutes = parseInt(document.getElementById('video-duration-minutes')?.value || '0', 10);
        const seconds = parseInt(document.getElementById('video-duration-seconds')?.value || '0', 10);
        newMaterial.DurationInSeconds = minutes * 60 + seconds;
    }

    const errors = validateMaterial(newMaterial);
    if (errors.length) {
        errors.forEach(err => showMaterialError(err.field, err.message));
        return;
    }

    materials.push(newMaterial);
    refreshMaterialList();

    document.getElementById('MaterialName').value = '';
    document.getElementById('material-type-selector').selectedIndex = 0;
    toggleMaterialFields(0);
    document.querySelectorAll('#new-material-form .material-type-fields input:not(#BookFile)').forEach(i => i.value = '');
}


function cancelNewMaterial() {
    const materialNameInput = document.getElementById('MaterialName');
    if (materialNameInput) materialNameInput.value = '';
    document.getElementById('material-type-selector').selectedIndex = 0;
    toggleMaterialFields(0);
    document.querySelectorAll('.material-type-fields input').forEach(i => i.value = '');
    refreshMaterialList();
}

function addExistingMaterial(button) {
    const dto = {
        MaterialId: parseInt(button.dataset.id) || 0,
        MaterialName: button.dataset.name || '',
        MaterialType: parseInt(button.dataset.type) || 0,
        BookTitle: button.dataset.title || '',
        Author: button.dataset.author || '',
        ResourceUrl: button.dataset.url || '',
        durationInSeconds: parseFloat(button.dataset.duration) || 0,
        MaterialOrder: materials.length,
        PublicationYear: parseInt(button.dataset.year) || 1901,
        Format: button.dataset.format || '',
        FilePath: button.dataset.filepath || '',
        NumberOfPages: parseInt(button.dataset.pages) || 2,
        Quality: button.dataset.quality || ''
    };
    const exists = materials.some(m => m.MaterialId === dto.MaterialId && dto.MaterialId !== 0);
    if (exists) { alert("This material is already added!"); return; }
    materials.push(dto);
    materials.forEach((m, i) => m.MaterialOrder = i);
    refreshMaterialList();
}

function deleteMaterial(index) {
    materials.splice(index, 1);
    materials.forEach((m, i) => m.MaterialOrder = i);
    refreshMaterialList();
}

function prepareFormData() {
    const form = document.getElementById('course-form');
    const formData = new FormData(form);

    materials.forEach((m, i) => {
        formData.append(`Materials[${i}].MaterialId`, m.MaterialId);
        formData.append(`Materials[${i}].MaterialName`, m.MaterialName);
        formData.append(`Materials[${i}].MaterialOrder`, m.MaterialOrder);
        formData.append(`Materials[${i}].MaterialType`, m.MaterialType);
        formData.append(`Materials[${i}].IsCompleted`, false);

        if (m.MaterialType === 0) {
            formData.append(`Materials[${i}].ResourceUrl`, m.ResourceUrl || '');
            formData.append(`Materials[${i}].DurationInSeconds`, m.DurationInSeconds || 0);
            formData.append(`Materials[${i}].Quality`, m.Quality || '');
        }

        if (m.MaterialType === 1) {
            formData.append(`Materials[${i}].ResourceUrl`, m.ResourceUrl || '');
            formData.append(`Materials[${i}].PublicationYear`, m.PublicationYear || '');
        }

        if (m.MaterialType === 2) {
            formData.append(`Materials[${i}].BookTitle`, m.BookTitle || '');
            formData.append(`Materials[${i}].Author`, m.Author || '');
            formData.append(`Materials[${i}].NumberOfPages`, m.NumberOfPages || '');
            formData.append(`Materials[${i}].PublicationYear`, m.PublicationYear || '');
            formData.append(`Materials[${i}].Format`, m.Format || '');
            if (m.BookFile) formData.append(`Materials[${i}].BookFile`, m.BookFile);
        }
    });

    skills.forEach((s, i) => {
        formData.append(`Skills[${i}].SkillId`, s.SkillId || 0);
        formData.append(`Skills[${i}].SkillName`, s.SkillName || '');
    });

    return formData;
}

function submitCourseForm(event) {
    event.preventDefault();
    const form = document.getElementById('course-form');
    const formData = prepareFormData();

    fetch(form.action, {
        method: form.method,
        body: formData
    })
        .then(response => {
            if (response.ok) {
                window.location.href = response.url;
            } else {
                console.error('Form submission failed.');
            }
        })
        .catch(err => console.error('Error submitting form:', err));
}

document.getElementById('course-form').addEventListener('submit', submitCourseForm);

function findInputByField(field) {
    const form = document.getElementById('new-material-form');
    if (!form) return null;
    let el = form.querySelector(`[name="${field}"]`) || form.querySelector(`#${field}`);
    if (el) return el;
    const normalizeToPascal = s => s.replace(/[-_ ]+([a-zA-Z0-9])/g, (m, c) => c.toUpperCase()).replace(/^./, c => c.toUpperCase());
    const pascal = normalizeToPascal(field);
    el = form.querySelector(`[name="${pascal}"]`) || form.querySelector(`#${pascal}`);
    if (el) return el;
    const camel = pascal.replace(/^./, c => c.toLowerCase());
    el = form.querySelector(`[name="${camel}"]`) || form.querySelector(`#${camel}`);
    if (el) return el;
    const kebab = pascal.replace(/([A-Z])/g, '-$1').toLowerCase().replace(/^-/, '');
    el = form.querySelector(`[name="${kebab}"]`) || form.querySelector(`#${kebab}`);
    return el;
}

const fieldMap = {
    VideoResourceUrl: "ResourceUrl",
    ArticleResourceUrl: "ResourceUrl",
    BookPublicationYear: "PublicationYear",
    ArticlePublicationYear: "PublicationYear",
};

function showMaterialError(fieldName, message) {
    const mappedField = fieldMap[fieldName] || fieldName;

    if (mappedField === 'MaterialName') {
        const nameField = document.querySelector('[name="MaterialName"]');
        if (nameField) nameField.classList.add('is-invalid');

        const nameFeedback = nameField.nextElementSibling;
        if (nameFeedback) nameFeedback.textContent = message;
        return;
    }

    if (mappedField === "Duration") {
        document.getElementById("video-duration-minutes")?.classList.add("is-invalid");
        document.getElementById("video-duration-seconds")?.classList.add("is-invalid");
    }

    const activeBlock = Array.from(document.querySelectorAll('#new-material-form .material-type-fields'))
        .find(div => div.offsetParent !== null);

    const field = activeBlock?.querySelector(`[name="${mappedField}"]`);
    if (field) field.classList.add("is-invalid");

    const validationSpan = activeBlock?.querySelector(`[data-valmsg-for="${mappedField}"]`);
    if (validationSpan) {
        validationSpan.textContent = message;
        validationSpan.classList.add("text-danger");
    } else if (field) {
        const feedback = activeBlock?.querySelector('span.text-danger:not([data-valmsg-for])'); 
        if (feedback) feedback.textContent = message;
    }
}

function clearMaterialErrors() {
    document.querySelectorAll('#new-material-form .form-control').forEach(input => {
        input.classList.remove('is-invalid');
    });

    document.querySelectorAll('#new-material-form span.text-danger').forEach(span => {
        span.textContent = '';
    });

    const durationFeedback = document.getElementById("duration-feedback");
    if (durationFeedback) {
        durationFeedback.textContent = '';
        durationFeedback.style.display = "none";
    }
}

function validateMaterial(material) {
    const errors = [];
    if (!material.MaterialName || !material.MaterialName.toString().trim()) {
        errors.push({ field: 'MaterialName', message: 'Material name is required' });
    }

    switch (Number(material.MaterialType)) {
        case 0:
            if (!material.DurationInSeconds || material.DurationInSeconds <= 0) {
                errors.push({ field: 'Duration', message: 'Duration is required' });
            }
            if (!material.ResourceUrl || !material.ResourceUrl.toString().trim()) {
                errors.push({ field: 'VideoResourceUrl', message: 'Video URL is required' });
            }
            if (!material.Quality || !material.Quality.toString().trim()) {
                errors.push({ field: 'Quality', message: 'Video quality is required' });
            }
            break;

        case 1:
            if (!material.ResourceUrl || !material.ResourceUrl.toString().trim()) {
                errors.push({ field: 'ArticleResourceUrl', message: 'Article URL is required' });
            }
            if (!material.PublicationYear || Number(material.PublicationYear) < 1900) {
                errors.push({ field: 'ArticlePublicationYear', message: 'Publication year is required and must be valid' });
            }
            break;

        case 2:
            if (!material.BookTitle || !material.BookTitle.toString().trim()) {
                errors.push({ field: 'BookTitle', message: 'Book title is required' });
            }
            if (!material.Author || !material.Author.toString().trim()) {
                errors.push({ field: 'Author', message: 'Book author is required' });
            }
            if (!material.PublicationYear || Number(material.PublicationYear) < 1900) {
                errors.push({ field: 'BookPublicationYear', message: 'Publication year is required and must be valid' });
            }
            if (!material.NumberOfPages || Number(material.NumberOfPages) <= 0) {
                errors.push({ field: 'NumberOfPages', message: 'Number of pages is required and must be greater than 0' });
            }
            if (!material.BookFile) {
                errors.push({ field: 'BookFile', message: 'Please upload the file.' });
            }
            if (material.BookFile && material.BookFile.name) {
                const allowedExtensions = ['doc', 'docx', 'pdf', 'txt', 'epub'];
                const fileName = material.BookFile.name;
                const fileExt = fileName.split('.').pop().toLowerCase();

                if (!allowedExtensions.includes(fileExt)) {
                    errors.push({ field: 'BookFile', message: `Invalid file type. Allowed types: ${allowedExtensions.join(', ')}` });
                }
            }
            break;
        default:
            errors.push({ field: 'MaterialType', message: 'Invalid material type' });
    }

    return errors;
}

function togglePassword(inputId, btn) {
    const input = document.getElementById(inputId);

    if (input.type === "password") {
        input.type = "text";
        btn.textContent = "Hide";
    } else {
        input.type = "password";
        btn.textContent = "Show";
    }
}

document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll(".alert[data-autohide]").forEach(alert => {
        const delay = parseInt(alert.dataset.delay || "3000", 10);
        setTimeout(() => {
            alert.classList.remove("show");
        }, delay);
    });
});

window.addExistingSkill = addExistingSkill;
window.addNewSkill = addNewSkill;
window.removeSkill = removeSkill;
window.saveNewMaterial = saveNewMaterial;
window.cancelNewMaterial = cancelNewMaterial;
window.addExistingMaterial = addExistingMaterial;
window.deleteMaterial = deleteMaterial;
window.refreshMaterialList = refreshMaterialList;
window.refreshSkillList = refreshSkillList;
window.togglePassword = togglePassword;
window.clearMaterialErrors = clearMaterialErrors;
window.showMaterialError = showMaterialError;
window.validateMaterial = validateMaterial;
window.prepareFormData = prepareFormData;
window.submitCourseForm = submitCourseForm;
window.toggleMaterialFields = toggleMaterialFields;
