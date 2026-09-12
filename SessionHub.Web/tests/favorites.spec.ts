import { expect, test } from '@playwright/test'

test.describe('Favorites workflow', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('http://localhost:4173')

    const favoriteSessionIds = await page.evaluate(async () => {
      const response = await fetch('/api/favorites')
      if (!response.ok) {
        return [] as number[]
      }

      const favorites = (await response.json()) as Array<{ id: number }>
      return favorites.map((favorite) => favorite.id)
    })

    for (const sessionId of favoriteSessionIds) {
      await page.evaluate(async (id) => {
        await fetch(`/api/favorites/${id}`, { method: 'DELETE' })
      }, sessionId)
    }

    await page.reload()
  })

  test('Mark session as favorite', async ({ page }) => {
    // Arrange
    await page.goto('http://localhost:4173')

    // Act
    const favoriteButton = page.locator('button[title="Add favorite"]').first()
    await favoriteButton.click()

    // Assert
    await expect(page.locator('button[title="Remove favorite"]').first()).toBeVisible()
    await expect(page.getByRole('button', { name: /Favorites \(\d+\)/ }).first()).toContainText('Favorites (1)')
  })

  test('View favorites page', async ({ page }) => {
    // Arrange
    await page.goto('http://localhost:4173')
    await page.locator('button[title="Add favorite"]').first().click()

    // Act
    await page.getByRole('button', { name: /Favorites \(\d+\)/ }).first().click()

    // Assert
    await expect(page.getByText('Favorite sessions')).toBeVisible()
    await expect(page.getByText('No favorites yet.')).not.toBeVisible()
    await expect(page.locator('.session-item')).toHaveCount(1)
  })

  test('Remove favorite', async ({ page }) => {
    // Arrange
    await page.goto('http://localhost:4173')
    await page.locator('button[title="Add favorite"]').first().click()
    await page.getByRole('button', { name: /Favorites \(\d+\)/ }).first().click()

    // Act
    await page.locator('button[title="Remove favorite"]').first().click()

    // Assert
    await expect(page.getByText('No favorites yet.')).toBeVisible()
    await expect(page.getByRole('button', { name: /Show all \(0\)/ }).first()).toBeVisible()
  })
})
