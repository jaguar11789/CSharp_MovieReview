const form    = document.getElementById("passwordChangeForm");
const button  = document.getElementById("changePasswordBtn");
const message = document.getElementById("passwordMessage");

form.addEventListener("submit", async (event) => {
    event.preventDefault();

    const currentPassword = document.getElementById("currentPassword").value;
    const newPassword     = document.getElementById("newPassword").value;
    const confirmPassword = document.getElementById("confirmPassword").value;

    // 비밀번호 확인
    if (newPassword !== confirmPassword) {
        message.className   = "password-message error";
        message.textContent = "새 비밀번호가 서로 일치하지 않습니다.";

        return;
    }

    if (currentPassword === newPassword) {
        message.className   = "password-message error";
        message.textContent = "현재 비밀번호와 다른 비밀번호를 입력해주세요.";

        return;
    }

    button.disabled = true;
    button.textContent = "변경 중...";

    try {
        const response = await fetch("https://localhost:7226/api/User/password", {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            credentials: "include",
            body: JSON.stringify({
                currentPassword: currentPassword,
                newPassword: newPassword
            })
        });

        const data = await response.json();

        if (!response.ok) {
            message.className = "password-message error";
            message.textContent = data.retMsg || "비밀번호 변경에 실패했습니다.";

            return;
        }

        if (data.retVal !== 0) {
            message.className   = "password-message error";
            message.textContent = data.retMsg || "비밀번호 변경에 실패했습니다.";

            return;
        }

        if (newPassword.length < 9 || newPassword.length > 18) {
            message.className   = "password-message error";
            message.textContent = "비밀번호는 9자리 이상 18자리 이하로 입력하세요.";

            return;
        }
        alert("비밀번호가 변경되었습니다. 다시 로그인해주세요.");

        try {
            await fetch("https://localhost:7226/api/Auth/logout", {
                method: "POST",
                credentials: "include"
            });
        }
        finally {
            window.location.href = "/accounts/login";
        }
        
    }
    catch (error) {
        console.error("비밀번호 변경 오류: ", error);

        message.className   = "password-message error";
        message.textContent = "서버와 통신하는 중 오류가 발생했습니다.";
    }
    finally {
        button.disabled    = false;
        button.textContent = "비밀번호 변경";
    }
});